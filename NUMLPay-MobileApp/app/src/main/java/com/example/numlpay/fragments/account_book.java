package com.example.numlpay.fragments;

import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import com.example.numlpay.R;
import com.example.numlpay.adapterclasses.AccountBookAdapter;
import com.example.numlpay.adapterclasses.ChallanAdapter;
import com.example.numlpay.api.ApiService;
import com.example.numlpay.network.ApiClient;
import com.example.numlpay.services.DialogUtils;

import java.util.ArrayList;
import java.util.List;

import model.UnPaidFeeView;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class account_book extends Fragment {


    private RecyclerView recyclerViewChallanList;
    private AccountBookAdapter accountBookAdapter;
    private List<UnPaidFeeView> challanList;
    private ProgressBar pBar;
    private String userId ;

    public account_book() {
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_account_book, container, false);
        userId = this.getArguments().getString("Id");
        pBar = view.findViewById(R.id.loader);
        pBar.setVisibility(View.VISIBLE);
        recyclerViewChallanList = view.findViewById(R.id.recyclerViewChallanList);
        recyclerViewChallanList.setLayoutManager(new LinearLayoutManager(getContext()));
        challanList = new ArrayList<>();
        accountBookAdapter = new AccountBookAdapter(getContext(), challanList);
        recyclerViewChallanList.setAdapter(accountBookAdapter);

        fetchChallanData(userId);

        return view;
    }

    private void fetchChallanData(String id) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);
        Call<List<UnPaidFeeView>> call = apiService.getAccountBook(id);
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



                    accountBookAdapter.notifyDataSetChanged();
                }
            }

            @Override
            public void onFailure(Call<List<UnPaidFeeView>> call, Throwable t) {
                // Handle failure
            }
        });
    }
}