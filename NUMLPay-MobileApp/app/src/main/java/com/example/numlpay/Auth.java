package com.example.numlpay;// Import necessary classes
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.example.numlpay.MainActivity;
import com.example.numlpay.api.ApiService;
import com.example.numlpay.network.ApiClient;
import com.example.numlpay.services.DialogUtils;
import com.google.gson.Gson;

import java.io.IOException;
import model.Users;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;

// Define the Auth class
public class Auth extends AppCompatActivity {

    // Define SharedPreferences
    private SharedPreferences sharedPreferences;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_auth);

        if (getSupportActionBar() != null) {
            getSupportActionBar().hide();
        }

        // Initialize SharedPreferences
        sharedPreferences = getSharedPreferences("user_session", Context.MODE_PRIVATE);

        // Check if user session exists
        if (isLoggedIn()) {
            goToMainActivity();
            return; // Exit onCreate to prevent further execution
        }

        // Initialize views
        EditText editTextUserID = findViewById(R.id.editTextUserID);
        EditText editTextPassword = findViewById(R.id.editTextPassword);
        ImageButton buttonShowPassword = findViewById(R.id.buttonShowPassword);
        TextView textViewForgotPassword = findViewById(R.id.textViewForgotPassword);
        Button buttonLogin = findViewById(R.id.buttonLogin);
        TextView textViewSignUp = findViewById(R.id.textViewSignUp);

        // Set OnClickListener for the login button
        buttonLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String userID = editTextUserID.getText().toString();
                String password = editTextPassword.getText().toString();
                if (userID.isEmpty() || password.isEmpty()) {
                    DialogUtils.showDialog(v.getContext(), "Error!", "Please fill all details.", 3);
                } else {
                    DialogUtils.showDialog(v.getContext(), "Signing In!", "Please wait...", 1);

                    Retrofit retrofit = ApiClient.getClient();
                    ApiService apiService = retrofit.create(ApiService.class);

                    Users user = new Users();
                    user.setNuml_id(userID);
                    user.setPassword(password);
                    Call<Users> call = apiService.loginUser(user);
                    call.enqueue(new Callback<Users>() {
                        @Override
                        public void onResponse(@NonNull Call<Users> call, @NonNull Response<Users> response) {
                            if (response.isSuccessful()) {
                                Users loggedUser = response.body();
                                DialogUtils.showDialog(v.getContext(), "Congratulations!", "Welcome Back " + loggedUser.getName(), 2);

                                // Save user session
                                saveSession(loggedUser);

                                // Proceed to the main activity
                                goToMainActivity();
                            } else {
                                String errorMessage;
                                if (response.errorBody() != null) {
                                    try {
                                        errorMessage = response.errorBody().string();
                                        Log.d("Error", errorMessage);
                                    } catch (IOException e) {
                                        errorMessage = "Error processing error message.";
                                        e.printStackTrace();
                                    }
                                } else {
                                    errorMessage = "Unknown error occurred.";
                                }
                                Log.d("Error", errorMessage);

                                DialogUtils.showDialog(v.getContext(), "Error!", errorMessage, 3);
                            }
                        }

                        @Override
                        public void onFailure(@NonNull Call<Users> call, @NonNull Throwable t) {
                            Log.e("API_CALL_FAILURE", "Error: " + t.getMessage());
                            Toast.makeText(getApplicationContext(), "Failed to connect to the server. Please check your internet connection.", Toast.LENGTH_SHORT).show();
                        }
                    });
                }
            }
        });

        // Set OnClickListener for the show/hide password button
        buttonShowPassword.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Toggle password visibility
                int inputType = editTextPassword.getInputType();
                if (inputType == 129) {
                    editTextPassword.setInputType(1);
                    buttonShowPassword.setImageResource(R.drawable.ic_eye_visibility);
                } else {
                    editTextPassword.setInputType(129);
                }
            }
        });

        // Set OnClickListener for the sign-up text view
        textViewSignUp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                // Append the dynamic part of the URL
                String dynamicPart = "/Home/Register";
                String fullUrl = getString(R.string.domain_url) + dynamicPart;

                // Open a web browser with the constructed URL
                Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(fullUrl));
                startActivity(intent);
            }
        });

        // Set OnClickListener for the forgot password text view
        textViewForgotPassword.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                // Append the dynamic part of the URL
                String dynamicPart = "/Home/forgotPasswordUser";
                String fullUrl = getString(R.string.domain_url) + dynamicPart;

                // Open a web browser with the constructed URL
                Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(fullUrl));
                startActivity(intent);            }
        });
    }

    // Method to check if user is logged in
    private boolean isLoggedIn() {
        // Check if the user ID is saved in SharedPreferences
        return sharedPreferences.contains("user");
    }

    // Method to save user session
    private void saveSession(Users user) {
        // Save the user ID in SharedPreferences
        SharedPreferences.Editor editor = sharedPreferences.edit();
        Gson gson = new Gson();
        String serializedUser = gson.toJson(user);
        editor.putString("user", serializedUser);
        editor.apply();
    }

    // Method to go to the main activity
    private void goToMainActivity() {
        Intent intent = new Intent(Auth.this, MainActivity.class);
        startActivity(intent);
        finish(); // Finish the Auth activity to prevent returning to it with the back button
    }
}
