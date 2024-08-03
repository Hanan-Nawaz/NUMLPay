package com.example.numlpay.services;

import android.content.Context;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.appcompat.app.AlertDialog;
import androidx.core.content.ContextCompat;

import com.example.numlpay.R;

public class DialogUtils {

    public static void showDialog(Context context, String title, String message, int isSuccess) {
        // Inflate the custom dialog layout

        if (context == null) {
            // Log an error or handle the null context case appropriately
            return;
        }

        ContextThemeWrapper themedContext = new ContextThemeWrapper(context, androidx.appcompat.R.style.Theme_AppCompat);


        LayoutInflater inflater = LayoutInflater.from(themedContext);
        View dialogView = inflater.inflate(R.layout.custom_dialoug, null);

        // Find views
        ImageView dialogIcon = dialogView.findViewById(R.id.dialogIcon);
        TextView dialogTitle = dialogView.findViewById(R.id.dialogTitle);
        TextView dialogMessage = dialogView.findViewById(R.id.dialogMessage);

        // Customize dialog based on success or error
        if (isSuccess == 1) {
            dialogIcon.setImageResource(R.drawable.loading);
        }
        else if (isSuccess == 2) {
            dialogIcon.setImageResource(R.drawable.sucess);
        }
        else {
            dialogIcon.setImageResource(R.drawable.fail);
        }

        dialogTitle.setText(title);
        dialogMessage.setText(message);

        // Create and show the dialog
        AlertDialog.Builder builder = new AlertDialog.Builder(themedContext);
        builder.setView(dialogView);
        AlertDialog dialog = builder.create();
        dialog.show();
    }
}
