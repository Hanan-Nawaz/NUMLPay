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

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

import model.BusRoute;
import model.EligibleFees;
import model.InstallmentManagement;
import model.ResponseMessage;
import model.SessionView;
import model.SpinnerItem;
import model.UnpaidFees;
import model.Users;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;


public class add_challan extends Fragment {

    private Spinner spinnerFeeFor, spinnerSession, spinnerBusRoute, spinnerInstallment;
    private Button buttonAddChallan;

    private SharedPreferences sharedPreferences;

    int feeFor, session, busRoute, installment;

    public add_challan() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_add_challan, container, false);
        sharedPreferences = requireContext().getSharedPreferences("user_session", Context.MODE_PRIVATE);

        String serializedUser = sharedPreferences.getString("user", "");
        Gson gson = new Gson();
        Users user = gson.fromJson(serializedUser, Users.class);

        spinnerFeeFor = view.findViewById(R.id.spinnerFeeFor);
        spinnerSession = view.findViewById(R.id.spinnerSession);
        spinnerBusRoute = view.findViewById(R.id.spinnerBusRoute);
        spinnerInstallment = view.findViewById(R.id.spinnerInstallment);
        buttonAddChallan = view.findViewById(R.id.buttonAddChallan);


        buttonAddChallan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {



                if (feeFor == 0 || session == 0 || installment == 0 || (feeFor == 2  && busRoute == 0)) {
                    DialogUtils.showDialog(v.getContext(), "Error!", "Please fill all details.", 3);
                }
                else {


                    Toast.makeText(getContext(), feeFor + " " +
                            session + " " +
                            installment, Toast.LENGTH_SHORT).show();

                    ApiService apiService = ApiClient.getClient().create(ApiService.class);

                    Date issueDateTime = new Date();

                    SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());

                    String formattedDate = sdf.format(issueDateTime);



                    if (feeFor == 2) {
                        UnpaidFees unpaidFee = new UnpaidFees();
                        unpaidFee.setFee_type(1);
                        unpaidFee.setStd_numl_id(user.getNuml_id());
                        unpaidFee.setSemester(user.getSemester());
                        unpaidFee.setIssue_date(formattedDate);
                        Call<Void> call = apiService.postChallan(
                                session,
                                feeFor,
                                installment,
                                user.getShift(),
                                user.getAdmission_session(),
                                user.getFee_plan(),
                                busRoute,
                                unpaidFee
                        );

                        call.enqueue(new Callback<Void>() {
                            @Override
                            public void onResponse(Call<Void> call, Response<Void> response) {

                                if (response.isSuccessful()) {





                                    // Show dialog with the success message
                                    DialogUtils.showDialog(v.getContext(), "Generated!", "Challan Generated Successfully!", 3);
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
                                Log.e("error", t.getMessage());
                            }
                        });
                    } else {
                        UnpaidFees unpaidFee = new UnpaidFees();
                        unpaidFee.setFee_type(1);
                        unpaidFee.setStd_numl_id(user.getNuml_id());
                        unpaidFee.setSemester(user.getSemester());
                        unpaidFee.setIssue_date(formattedDate);

                        Call<Void> call = apiService.generateChallan(
                                session,
                                feeFor,
                                installment,
                                user.getShift(),
                                user.getAdmission_session(),
                                user.getFee_plan(),
                                user.getDepartment(),
                                unpaidFee
                        );

                        call.enqueue(new Callback<Void>() {
                            @Override
                            public void onResponse(Call<Void> call, Response<Void> response) {

                                if (response.isSuccessful()) {

                                    // Show dialog with the success message
                                    DialogUtils.showDialog(v.getContext(), "Generated!", "Challan Generated Successfully!", 3);
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
//
                            }

                            @Override
                            public void onFailure(Call<Void> call, Throwable t) {
                                // Handle failure, e.g., network error
                                Log.e("error", t.getMessage());
                            }
                        });
                    }

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

                populateSpinnerInstallment(feeFor);
            }

            @Override
            public void onNothingSelected(AdapterView<?> parentView) {
                // Do nothing if nothing is selected
            }
        });

        spinnerBusRoute.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parentView, View selectedItemView, int position, long id) {
                // Call the method to populate spinnerInstallment when spinnerFeeFor selection changes
                SpinnerItem selectedItem = (SpinnerItem) parentView.getItemAtPosition(position);

                // Show Toast with the ID associated with the selected text
                if (selectedItem != null) {
                    busRoute = selectedItem.getId();
                }              }

            @Override
            public void onNothingSelected(AdapterView<?> parentView) {
                // Do nothing if nothing is selected
            }
        });

        spinnerInstallment.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parentView, View selectedItemView, int position, long id) {
                // Call the method to populate spinnerInstallment when spinnerFeeFor selection changes
                SpinnerItem selectedItem = (SpinnerItem) parentView.getItemAtPosition(position);

                // Show Toast with the ID associated with the selected text
                if (selectedItem != null) {
                    installment = selectedItem.getId();
                }            }

            @Override
            public void onNothingSelected(AdapterView<?> parentView) {
                // Do nothing if nothing is selected
            }
        });

        spinnerSession.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parentView, View selectedItemView, int position, long id) {
                // Call the method to populate spinnerInstallment when spinnerFeeFor selection changes
                SpinnerItem selectedItem = (SpinnerItem) parentView.getItemAtPosition(position);

                // Show Toast with the ID associated with the selected text
                if (selectedItem != null) {
                    session = selectedItem.getId();
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parentView) {
                // Do nothing if nothing is selected
            }
        });


        populateSpinnerFeeFor(user);
        populateSpinnerSession(user);

        populateSpinnerBusRoute(user.getDepartment());

        return  view;
    }

    private void populateSpinnerFeeFor(Users user) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);
        EligibleFees eFee = new EligibleFees();
        eFee.setStd_numl_id(user.getNuml_id());
        Call<EligibleFees> call = apiService.getEligibleFees(eFee);
        call.enqueue(new Callback<EligibleFees>() {
            @Override
            public void onResponse(Call<EligibleFees> call, Response<EligibleFees> response) {
                if (response.isSuccessful()) {
                    EligibleFees eFees = response.body();
                    List<SpinnerItem> feeOptions = new ArrayList<>();

                    feeOptions.add(new SpinnerItem(0, "Select Fee Type"));

                    feeOptions.add(new SpinnerItem(1, "Tuition Fee"));

                    if (eFees != null && eFees.getBus_fee() == 1) {
                        feeOptions.add(new SpinnerItem(2, "Bus Fee"));
                    }

                    if (eFees != null && eFees.getHostel_fee() == 1) {
                        feeOptions.add(new SpinnerItem(3, "Hostel Fee"));
                        feeOptions.add(new SpinnerItem(4, "Mess Fee"));
                    }

                    // Create an ArrayAdapter for the spinner
                    ArrayAdapter<SpinnerItem> adapter = new ArrayAdapter<>(requireContext(), R.layout.custom_spinner_item, feeOptions);
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);

                    // Set the adapter to the spinner
                    adapter.notifyDataSetChanged();
                    spinnerFeeFor.setAdapter(adapter);
                } else {
                    // Handle unsuccessful response
                }
            }

            @Override
            public void onFailure(Call<EligibleFees> call, Throwable t) {
                // Handle failure
            }
        });
    }


    private void populateSpinnerSession(Users users) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);

        Call<List<SessionView>> call = apiService.getSessionForDropdown(users.getAdmission_session());
        call.enqueue(new Callback<List<SessionView>>() {
            @Override
            public void onResponse(Call<List<SessionView>> call, Response<List<SessionView>> response) {
                if (response.isSuccessful()) {
                    List<SessionView> sessionList = response.body();

                    List<SpinnerItem> spinnerItems = new ArrayList<>();
                    spinnerItems.add(new SpinnerItem(0, "Select Fee Session"));

                    if (sessionList != null) {
                        for(int i = 0; i < sessionList.size(); i++){
                            spinnerItems.add(new SpinnerItem(sessionList.get(i).getId(), sessionList.get(i).getSession()));
                        }
                    }

                    ArrayAdapter<SpinnerItem> adapter = new ArrayAdapter<>(requireContext(), R.layout.custom_spinner_item, spinnerItems);
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);

                    // Set the adapter to the spinner
                    adapter.notifyDataSetChanged();
                    spinnerSession.setAdapter(adapter);
                } else {
                    // Handle unsuccessful response
                }
            }

            @Override
            public void onFailure(Call<List<SessionView>> call, Throwable t) {
                // Handle failure
            }
        });
    }


    private void populateSpinnerBusRoute(int departmentId) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);
        Call<List<BusRoute>> call = apiService.getActiveBusRoutes(departmentId);
        call.enqueue(new Callback<List<BusRoute>>() {
            @Override
            public void onResponse(Call<List<BusRoute>> call, Response<List<BusRoute>> response) {
                if (response.isSuccessful()) {
                    List<BusRoute> busRoutes = response.body();
                    List<SpinnerItem> spinnerItems = new ArrayList<>();
                    spinnerItems.add(new SpinnerItem(0, "Select Bus Route (Only for Bus Fee)"));

                    if (busRoutes != null) {
                        for (BusRoute busRoute : busRoutes) {
                            spinnerItems.add(new SpinnerItem(busRoute.getId(), busRoute.getName()));
                        }
                    }

                    ArrayAdapter<SpinnerItem> adapter = new ArrayAdapter<>(requireContext(), R.layout.custom_spinner_item, spinnerItems);
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);

                    // Set the adapter to the spinner
                    adapter.notifyDataSetChanged();
                    spinnerBusRoute.setAdapter(adapter);
                } else {
                    // Handle unsuccessful response
                }
            }

            @Override
            public void onFailure(Call<List<BusRoute>> call, Throwable t) {
                // Handle failure
            }
        });
    }


    private void populateSpinnerInstallment(int feeFor) {
        ApiService apiService = ApiClient.getClient().create(ApiService.class);

        Call<List<InstallmentManagement>> call = apiService.getActiveInstallments(feeFor);
        call.enqueue(new Callback<List<InstallmentManagement>>() {
            @Override
            public void onResponse(Call<List<InstallmentManagement>> call, Response<List<InstallmentManagement>> response) {
                if (response.isSuccessful()) {
                    List<InstallmentManagement> installmentList = response.body();
                    List<SpinnerItem> spinnerItems = new ArrayList<>();
                    spinnerItems.add(new SpinnerItem(0, "Select Fee Installment"));

                    if (installmentList != null) {
                        for (InstallmentManagement installment : installmentList) {
                            String name = getName(installment.getMode());
                            spinnerItems.add(new SpinnerItem(installment.getInstallment_id(), name));
                        }
                    }


                    ArrayAdapter<SpinnerItem> adapter = new ArrayAdapter<>(requireContext(), R.layout.custom_spinner_item, spinnerItems);
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);

                    // Set the adapter to the spinner
                    adapter.notifyDataSetChanged();
                    spinnerInstallment.setAdapter(adapter);
                    // Populate spinnerInstallment with the received data
                } else {
                    // Handle error response
                }
            }

            @Override
            public void onFailure(Call<List<InstallmentManagement>> call, Throwable t) {
                // Handle network failure
            }
        });
    }

    private String getName(int value) {
        switch (value) {
            case 1:
                return "No Installments";
            case 2:
                return "2 Installments";
            case 3:
                return "3 Installments";
            case 4:
                return "4 Installments";
            default:
                return "unknown";
        }
    }


}