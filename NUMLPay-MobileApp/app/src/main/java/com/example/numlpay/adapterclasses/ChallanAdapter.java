package com.example.numlpay.adapterclasses;

import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.content.Context;
import android.graphics.Bitmap;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.FragmentManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.numlpay.R;
import com.example.numlpay.api.ApiService;
import com.example.numlpay.network.ApiClient;
import com.example.numlpay.services.DialogUtils;
import com.example.numlpay.services.DownloadFileTask;

import java.io.IOException;
import java.util.List;

import model.AccountBook;
import model.EmailModel;
import model.UnPaidFeeView;
import model.Users;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChallanAdapter extends RecyclerView.Adapter<ChallanAdapter.ChallanViewHolder> {

    private Context context;
    private List<UnPaidFeeView> challanList;
    private FragmentManager fragmentManager;


    Users user;

    public ChallanAdapter(Context context, List<UnPaidFeeView> challanList, Users user, FragmentManager fragmentManager) {
        this.context = context;
        this.challanList = challanList;
        this.user = user;
        this.fragmentManager = fragmentManager;
    }

    @NonNull
    @Override
    public ChallanViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_challan, parent, false);
        return new ChallanViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ChallanViewHolder holder, int position) {
        UnPaidFeeView challan = challanList.get(position);
        holder.bind(challan);
    }

    @Override
    public int getItemCount() {
        return challanList.size();
    }

    public void setData(List<UnPaidFeeView> dataList) {
        challanList.clear();
        challanList.addAll(dataList);
        notifyDataSetChanged();
    }

    public class ChallanViewHolder extends RecyclerView.ViewHolder {
        TextView textViewChallanNo, textViewChallanType, textViewTotalFee, textViewDueDate;
        Button buttonPay, buttonPrint;



        public ChallanViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewChallanNo = itemView.findViewById(R.id.textViewChallanNo);
            textViewChallanType = itemView.findViewById(R.id.textViewChallanType);
            textViewTotalFee = itemView.findViewById(R.id.textViewTotalFee);
            textViewDueDate = itemView.findViewById(R.id.textViewDueDate);
            buttonPay = itemView.findViewById(R.id.buttonPay);
            buttonPrint = itemView.findViewById(R.id.buttonPrint);

        }

        public void bind(UnPaidFeeView challan) {
            textViewChallanNo.setText(String.valueOf(challan.getChallan_no()));
            textViewChallanType.setText(challan.getFeeFor());
            textViewTotalFee.setText(String.valueOf(challan.getTotal_fee()));
            textViewDueDate.setText(challan.getDue_date());



            buttonPay.setOnClickListener(v -> {
                if ("Required Verification".equals(challan.getStatus())) {
                    Toast.makeText(context, "Challan Under verification phase. Kindly wait.", Toast.LENGTH_SHORT).show();
                } else if ("fee Arrears".equals(challan.getStatus())) {
                    Toast.makeText(context, "Challan is added to fee Arrears.", Toast.LENGTH_SHORT).show();
                } else {
                    DialogUtils.showDialog(context, "Loading!", "Please wait!." , 1);
                    showWebViewDialog(context.getString(R.string.domain_url) + "/Main/Online?Id="+ challan.getId() +"&feeType=" + challan.getFee_type() + "&fee_for=" + challan.getFee_for() , challan.getId(), user.getNuml_id(), user.getEmail());
                }
            });

            buttonPrint.setOnClickListener(v -> {
                DialogUtils.showDialog(context, "Downloading!", "Please wait!." , 1);
                String downloadUrl = context.getString(R.string.domain_url) + "/Main/DownloadPdf?Id=" + challan.getId() + "&feeType=" + challan.getFee_type() + "&fee_for=" + challan.getFee_for();
                new DownloadFileTask(v.getContext()).execute(downloadUrl);
            });
        }

        @SuppressLint("SetJavaScriptEnabled")
        private void showWebViewDialog(String url, int id, String userId, String email) {
            String SUCCESS_URL = context.getString(R.string.domain_url) + "//successPage";
            String CANCEL_URL = context.getString(R.string.domain_url) + "//Dashboard";
            Log.d("url", url);
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            WebView webView = new WebView(context);
            webView.getSettings().setJavaScriptEnabled(true);

            final AlertDialog[] dialog = {null}; // Declare the dialog array

            webView.setWebViewClient(new WebViewClient() {
                @Override
                public void onPageStarted(WebView view, String url, Bitmap favicon) {
                    super.onPageStarted(view, url, favicon);
                    if (url.equals(SUCCESS_URL)) {
                        handleSuccess(context, id, userId, email);
                        if (dialog[0] != null) {
                            dialog[0].dismiss();
                        }
                    } else if (url.equals(CANCEL_URL)) {
                        if (dialog[0] != null) {
                            dialog[0].dismiss();
                        }
                        handleCancel(context);
                    }
                }
            });

            webView.loadUrl(url);
            builder.setView(webView);
            builder.setPositiveButton("X", (dialogs, which) -> dialogs.dismiss());
            dialog[0] = builder.create(); // Assign the created dialog to the array
            dialog[0].show();
        }

        private void handleSuccess(Context context, int id, String userId, String email) {
            ApiService apiService = ApiClient.getClient().create(ApiService.class);

            AccountBook accountBook = new AccountBook();
            accountBook.setChallan_no(id);
            accountBook.setStd_numl_id(userId);

            Call<Void> call = apiService.postAccountBook(accountBook);

            call.enqueue(new Callback<Void>() {
                @Override
                public void onResponse(Call<Void> call, Response<Void> response) {
                    if (response.isSuccessful()) {

                        String body = "Dear Student,\r\n\r\nWe are delighted to inform you that your" +
                                " academic fee payment has been successfully processed through NUMLPay, our secure and user-friendly" +
                                " payment platform. This email serves as an official confirmation of your payment for the semester/term " +
                                "\r\n\r\nWith your fee now successfully paid, you can seamlessly continue with" +
                                " your academic pursuits, secure in the knowledge that your enrollment remains uninterrupted and that you have " +
                                "full access to the wide array of resources that NUML provides.\r\n\r\nShould you have any queries or require further " +
                                "assistance related to your payment or any other aspect, please do not hesitate to get in touch with our dedicated" +
                                " support team at numlfeepay@gmail.com. We are here to provide you with the assistance " +
                                "you may need.\r\n\r\nYour confidence in NUMLPay is greatly appreciated, and we eagerly anticipate the opportunity to " +
                                "contribute to your ongoing academic journey.\r\n\r\nBest regards,\r\n\r\nNUMLPay" +
                                "\r\n\r\nNote: This email is for informational purposes only. No further action is required on your part.\r\n";

                        EmailModel emailModel = new EmailModel();
                        emailModel.setReceiverEmail(email);
                        emailModel.setBody(body);
                        emailModel.setSubject("Confirmation of Payment - Your Fee is Paid through NUMLPay");

                        Call<Void> callEmail = apiService.sendEmail(emailModel);

                        callEmail.enqueue(new Callback<Void>() {
                            @Override
                            public void onResponse(Call<Void> callEmail, Response<Void> response) {
                                if (response.isSuccessful()) {

                                    // Show dialog with the success message
                                    DialogUtils.showDialog(context, "Congrats!", "Fee Paid Successfully!", 2);
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
                                    DialogUtils.showDialog(context, "Error!", errorMessage, 3);
                                }
                            }

                            @Override
                            public void onFailure(Call<Void> callEmail, Throwable t) {
                                // Handle failure, e.g., network error
                                // For example, show a toast message with the error
                                Toast.makeText(context, "Failed to generate challan: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                            }
                        });
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
                        DialogUtils.showDialog(context, "Error!", errorMessage, 3);
                    }
                }

                @Override
                public void onFailure(Call<Void> call, Throwable t) {
                    // Handle failure, e.g., network error
                    // For example, show a toast message with the error
                    Toast.makeText(context, "Failed to generate challan: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        }

        private void handleCancel(Context context) {
            DialogUtils.showDialog(context, "OOPS!", "Unable to Pay Fee Online." , 3);
        }
    }
}
