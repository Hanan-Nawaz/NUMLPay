package com.example.numlpay.fragments;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.Toast;

import com.example.numlpay.R;
import com.example.numlpay.api.ApiService;
import com.example.numlpay.network.ApiClient;
import com.example.numlpay.services.DialogUtils;
import com.google.gson.Gson;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

import model.EligibleFees;
import model.MiscellaneousFeeItem;
import model.ResponseMessage;
import model.SpinnerItem;
import model.UnpaidFees;
import model.Users;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class other_fee extends Fragment {

    private Spinner spinnerFeeFor;
    private Button buttonAddChallan;

    private SharedPreferences sharedPreferences;

    int feeFor;

    public other_fee() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_other_fee, container, false);
        sharedPreferences = requireContext().getSharedPreferences("user_session", Context.MODE_PRIVATE);

        String serializedUser = sharedPreferences.getString("user", "");
        Gson gson = new Gson();
        Users user = gson.fromJson(serializedUser, Users.class);

        spinnerFeeFor = view.findViewById(R.id.spinnerFeeFor);
        buttonAddChallan = view.findViewById(R.id.buttonAddChallan);


        buttonAddChallan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {


                if (feeFor == 0) {
                    DialogUtils.showDialog(v.getContext(), "Error!", "Please fill all details.", 3);
                } else {



                    ApiService apiService = ApiClient.getClient().create(ApiService.class);

                    Date issueDateTime = new Date();

                    SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());

                    String formattedDate = sdf.format(issueDateTime);


                        UnpaidFees unpaidFee = new UnpaidFees();
                        unpaidFee.setFee_type(2);
                        unpaidFee.setStd_numl_id(user.getNuml_id());
                        unpaidFee.setSemester(user.getSemester());
                        unpaidFee.setIssue_date(formattedDate);
                        unpaidFee.setFee_id(feeFor);

                    Call<Void> call = apiService.generateChallan(unpaidFee);

                    call.enqueue(new Callback<Void>() {
                        @Override
                        public void onResponse(Call<Void> call, Response<Void> response) {
                            if (response.isSuccessful()) {

                                // Show dialog with the success message
                                DialogUtils.showDialog(v.getContext(), "Generated!", "Challan Generated Successfully!", 2);
                            }
                            else{
                                String errorMessage;
                                try {
                                    errorMessage = response.errorBody().string(); // Get the error message from the response body
                                } catch (IOException e) {
                                    e.printStackTrace();
                                    errorMessage = "Failed to get error message"; // Default error message if unable to read error body
                                }

                                // Log the error message
                                Log.e("API_ERROR", errorMessage);

                                // Show dialog with the error message
                                DialogUtils.showDialog(v.getContext(), "Error!", errorMessage, 3);
                            }
                        }

                        @Override
                        public void onFailure(Call<Void> call, Throwable t) {
                            // Handle failure, e.g., network error
                            // For example, show a toast message with the error
                            Toast.makeText(getContext(), "Failed to generate challan: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                    });


                }

            }
        });

        spinnerFeeFor.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parentView, View selectedItemView, int position, long id) {
                // Call the method to populate spinnerInstallment when spinnerFeeFor selection changes
                SpinnerItem selectedItem = (SpinnerItem) parentView.getItemAtPosition(position);

                // Show Toast with the ID associated with the selected text
                if (selectedItem != null) {
                    feeFor = selectedItem.getId();
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parentView) {
                // Do nothing if nothing is selected
            }
        });

        populateSpinnerFeeFor(user);

        return view;
    }

    private void populateSpinnerFeeFor(Users user) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);
        // Make API call
        apiService.getMiscellaneousFees().enqueue(new Callback<JsonArray>() {
            @Override
            public void onResponse(Call<JsonArray> call, Response<JsonArray> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<SpinnerItem> itemList = new ArrayList<>();
                    itemList.add(new SpinnerItem(0, "Select Fee Type"));

                    // Parse the JSON array
                    JsonArray jsonArray = response.body();
                    for (JsonElement element : jsonArray) {
                        JsonObject jsonObject = element.getAsJsonObject();
                        int id = jsonObject.get("Id").getAsInt();
                        String name = jsonObject.get("name").getAsString();
                        itemList.add(new SpinnerItem(id, name));
                    }

                    // Populate the spinner with the data
                    ArrayAdapter<SpinnerItem> adapter = new ArrayAdapter<>(getContext(), R.layout.custom_spinner_item, itemList);
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spinnerFeeFor.setAdapter(adapter);
                }
            }

            @Override
            public void onFailure(Call<JsonArray> call, Throwable t) {
                // Handle failure, e.g., network error
                // Show error message to the user
            }
        });

    }
}