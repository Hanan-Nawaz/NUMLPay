package com.example.numlpay.services;

import android.content.Context;
import android.os.AsyncTask;
import android.os.Environment;
import android.util.Log;

import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;

public class DownloadFileTask extends AsyncTask<String, Void, Void> {

    private static final String TAG = "DownloadFileTask";
    private Context context;

    public DownloadFileTask(Context context) {
        this.context = context;
    }

    @Override
    protected Void doInBackground(String... urls) {
        String fileUrl = urls[0];
        String fileName = "challan.pdf";
        try {
            URL url = new URL(fileUrl);
            HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setRequestMethod("GET");
            urlConnection.connect();

            // Check if response code is OK (200)
            if (urlConnection.getResponseCode() == HttpURLConnection.HTTP_OK) {
                File dir = new File(Environment.getExternalStorageDirectory().getAbsolutePath() + "/");
                if (!dir.exists()) {
                    dir.mkdirs();
                }
                File file = new File(dir, fileName);
                FileOutputStream fileOutput = new FileOutputStream(file);

                InputStream inputStream = urlConnection.getInputStream();

                byte[] buffer = new byte[1024];
                int bufferLength;
                while ((bufferLength = inputStream.read(buffer)) > 0) {
                    fileOutput.write(buffer, 0, bufferLength);
                }
                fileOutput.close();
                inputStream.close();
                // Don't perform UI operations here
            } else {
                Log.e(TAG, "Server returned HTTP " + urlConnection.getResponseCode() + " " + urlConnection.getResponseMessage());
            }
            urlConnection.disconnect();
        } catch (Exception e) {
            Log.e(TAG, "Error downloading file: " + e.getMessage());
            e.printStackTrace();
        }
        return null;
    }

    @Override
    protected void onPostExecute(Void aVoid) {
        super.onPostExecute(aVoid);
        // Perform UI operations in onPostExecute, which runs on the main (UI) thread
        showDialog("Success!","Challan Downloaded Successfully!");
    }

    private void showDialog(String title, String message) {
        // Use context to show the dialog
        DialogUtils.showDialog(context, title, message, 2);
    }
}
