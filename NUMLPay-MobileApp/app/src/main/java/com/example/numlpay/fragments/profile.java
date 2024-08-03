package com.example.numlpay.fragments;

import android.content.Context;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import com.example.numlpay.R;
import com.google.gson.Gson;
import com.squareup.picasso.Picasso;

import model.Users;

public class profile extends Fragment {

    private TextView tv_name, tv_email, tv_contact, tv_cnic, tv_numl_id, tv_father_name, tv_dob;

    private ImageView iv_profile;
    private SharedPreferences sharedPreferences;

    public profile() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_profile, container, false);

        sharedPreferences = requireContext().getSharedPreferences("user_session", Context.MODE_PRIVATE);

        String serializedUser = sharedPreferences.getString("user", "");
        Gson gson = new Gson();
        Users user = gson.fromJson(serializedUser, Users.class);

        tv_name = view.findViewById(R.id.tv_name);
        tv_email = view.findViewById(R.id.tv_email);
        tv_contact = view.findViewById(R.id.tv_contact);
        tv_cnic = view.findViewById(R.id.tv_cnic);
        tv_numl_id = view.findViewById(R.id.tv_numl_id);
        tv_father_name = view.findViewById(R.id.tv_father_name);
        tv_dob = view.findViewById(R.id.tv_dob);
        iv_profile = view.findViewById(R.id.iv_profile);

        Uri imageUri = Uri.parse(getString(R.string.domain_url)).buildUpon().appendPath(user.getImage()).build();
        String imageUrl = imageUri.toString();
        Picasso.get()
                .load(imageUrl)
                .placeholder(R.drawable.loading)
                .error(R.drawable.fail)
                .into(iv_profile);

        tv_name.setText(user.getName());
        tv_email.setText(user.getEmail());
        tv_contact.setText(user.getContact());
        tv_cnic.setText(user.getNic());
        tv_numl_id.setText(user.getNuml_id());
        tv_father_name.setText(user.getFather_name());
        tv_dob.setText(user.getDate_of_birth());



        return  view;
    }
}