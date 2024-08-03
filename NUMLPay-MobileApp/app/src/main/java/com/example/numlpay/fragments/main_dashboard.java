package com.example.numlpay.fragments;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.numlpay.R;
import com.example.numlpay.adapterclasses.ChallanAdapter;
import com.example.numlpay.api.ApiService;
import com.example.numlpay.network.ApiClient;
import com.example.numlpay.services.DialogUtils;
import com.google.gson.Gson;

import java.util.ArrayList;
import java.util.List;

import model.UnPaidFeeView;
import model.Users;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class main_dashboard extends Fragment {

    private RecyclerView recyclerViewChallanList;
    private ChallanAdapter challanAdapter;
    private List<UnPaidFeeView> challanList;

    private SharedPreferences sharedPreferences;

    private ProgressBar pBar;

    private String userId ;

    public main_dashboard() {
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_main_dashboard, container, false);
        userId = this.getArguments().getString("Id");

        sharedPreferences = requireContext().getSharedPreferences("user_session", Context.MODE_PRIVATE);

        String serializedUser = sharedPreferences.getString("user", "");
        Gson gson = new Gson();
        Users user = gson.fromJson(serializedUser, Users.class);

        recyclerViewChallanList = view.findViewById(R.id.recyclerViewChallanList);
        pBar = view.findViewById(R.id.loader);
        pBar.setVisibility(View.VISIBLE);
        recyclerViewChallanList.setLayoutManager(new LinearLayoutManager(getContext()));
        challanList = new ArrayList<>();
        challanAdapter = new ChallanAdapter(getContext(), challanList, user, getFragmentManager());
        recyclerViewChallanList.setAdapter(challanAdapter);

        fetchChallanData(userId);

        return view;
    }

    private void fetchChallanData(String id) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);
        Call<List<UnPaidFeeView>> call = apiService.getUnPaidFees(id);
        call.enqueue(new Callback<List<UnPaidFeeView>>() {
            @Override
            public void onResponse(Call<List<UnPaidFeeView>> call, Response<List<UnPaidFeeView>> response) {
                if (response.isSuccessful()) {
                    if(response.body() == null){
                        pBar.setVisibility(View.GONE);
                        DialogUtils.showDialog(getContext(), "Oops!", "No data found!", 3);
                    }
                    else{
                        pBar.setVisibility(View.GONE);
                        challanList.addAll(response.body());
                        Log.d("here", challanList.toString());
                    }
                }
            }

            @Override
            public void onFailure(Call<List<UnPaidFeeView>> call, Throwable t) {
                // Handle failure
            }
        });
    }
}
