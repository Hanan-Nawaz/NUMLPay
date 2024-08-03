package com.example.numlpay.adapterclasses;


import android.content.Context;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.numlpay.R;
import com.example.numlpay.services.DialogUtils;
import com.example.numlpay.services.DownloadFileTask;

import java.util.List;

import model.UnPaidFeeView;

public class AccountBookAdapter extends RecyclerView.Adapter<AccountBookAdapter.ChallanViewHolder> {

    private Context context;
    private List<UnPaidFeeView> challanList;

    public AccountBookAdapter(Context context, List<UnPaidFeeView> challanList) {
        this.context = context;
        this.challanList = challanList;
    }

    @NonNull
    @Override
    public ChallanViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_account_book, parent, false);
        return new ChallanViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ChallanViewHolder holder, int position) {

            UnPaidFeeView challan = challanList.get(position);
            holder.bind(challan);
    }

    @Override
    public int getItemCount() {
        return  challanList.size();
    }

    public void setData(List<UnPaidFeeView> dataList) {
        challanList.clear();
        challanList.addAll(dataList);
        notifyDataSetChanged();
    }

    public class ChallanViewHolder extends RecyclerView.ViewHolder {
        TextView textViewChallanNo, textViewChallanType, textViewTotalFee, textViewDueDate;
        Button buttonPay;

        public ChallanViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewChallanNo = itemView.findViewById(R.id.textViewChallanNo);
            textViewChallanType = itemView.findViewById(R.id.textViewChallanType);
            textViewTotalFee = itemView.findViewById(R.id.textViewTotalFee);
            textViewDueDate = itemView.findViewById(R.id.textViewDueDate);
            buttonPay = itemView.findViewById(R.id.buttonPay);
        }

        public void bind(UnPaidFeeView challan) {
            textViewChallanNo.setText(String.valueOf(challan.getChallan_no()));
            textViewChallanType.setText(challan.getFeeFor());
            textViewTotalFee.setText(String.valueOf(challan.getTotal_fee()));
            textViewDueDate.setText(challan.getDue_date());

            buttonPay.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    DialogUtils.showDialog(context, "Downloading!", "Please wait!." , 1);

                    String downloadUrl = context.getString(R.string.domain_url) + "/AccountBook/DownloadPdf?Id=" + challan.getId() + "&feeType=" + challan.getFee_type();
                    new DownloadFileTask(v.getContext()).execute(downloadUrl);
                }
            });
        }

    }
}
