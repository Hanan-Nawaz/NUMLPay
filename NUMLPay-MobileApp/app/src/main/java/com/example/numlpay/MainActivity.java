package com.example.numlpay;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentTransaction;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.example.numlpay.fragments.account_book;
import com.example.numlpay.fragments.add_challan;
import com.example.numlpay.fragments.main_dashboard;
import com.example.numlpay.fragments.other_fee;
import com.example.numlpay.fragments.profile;
import com.google.gson.Gson;
import com.squareup.picasso.Picasso;
import androidx.appcompat.widget.Toolbar;

import com.google.android.material.navigation.NavigationView;

import model.Users;

public class MainActivity extends AppCompatActivity {

    private SharedPreferences sharedPreferences;

    Toolbar toolbar;
    DrawerLayout drawerLayout;
    ActionBarDrawerToggle toggle;
    NavigationView navigationView;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        toolbar = findViewById(R.id.toolbar);
        drawerLayout = findViewById(R.id.drawerLayout);
        navigationView = findViewById(R.id.nav);

        sharedPreferences = getSharedPreferences("user_session", Context.MODE_PRIVATE);

        View headerView = navigationView.getHeaderView(0);
        ImageView imageViewHeader = headerView.findViewById(R.id.tv_std_image);
        TextView tvName = headerView.findViewById(R.id.tv_std_name);
        TextView tv_title = toolbar.findViewById(R.id.Title);

        String serializedUser = sharedPreferences.getString("user", "");
        Gson gson = new Gson();
        Users user = gson.fromJson(serializedUser, Users.class);

        if (user != null && !TextUtils.isEmpty(user.getImage())) {
            Uri imageUri = Uri.parse(getString(R.string.domain_url)).buildUpon().appendPath(user.getImage()).build();
            String imageUrl = imageUri.toString();
            tvName.setText(user.getName());
            Picasso.get()
                    .load(imageUrl)
                    .placeholder(R.drawable.loading)
                    .error(R.drawable.fail)
                    .into(imageViewHeader);
        }
        else{

        }

        if(savedInstanceState == null){
            tv_title.setText("Dashboard");
            Bundle bundle = new Bundle();
            bundle.putString("Id", user.getNuml_id() );
            main_dashboard dashboard = new main_dashboard();
            dashboard.setArguments(bundle);
            bundle.putInt("15", 0);
            getSupportFragmentManager().beginTransaction()
                    .setReorderingAllowed(true)
                    .add(R.id.Main, main_dashboard.class, bundle).commit();
        }

        setSupportActionBar(toolbar);
        toggle = new ActionBarDrawerToggle(this,drawerLayout, toolbar, R.string.open, R.string.close);
        toggle.getDrawerArrowDrawable().setColor(getResources().getColor(R.color.white));
        drawerLayout.addDrawerListener(toggle);

        toggle.syncState();


        navigationView.setNavigationItemSelectedListener(new NavigationView.OnNavigationItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected(@NonNull MenuItem item) {
                if (item != null) {
                    int itemId = item.getItemId();

                    if (itemId == R.id.dashboard) {
                        tv_title.setText("Dashboard");
                        Bundle bundle = new Bundle();
                        bundle.putString("Id", user.getNuml_id() );
                        main_dashboard dashboard = new main_dashboard();
                        dashboard.setArguments(bundle);
                        FragmentTransaction fragmentTransaction = getSupportFragmentManager().beginTransaction();
                        fragmentTransaction.replace(R.id.Main, dashboard);
                        fragmentTransaction.commit();
                        Toast.makeText(getApplicationContext(), "Dashboard", Toast.LENGTH_LONG).show();
                        drawerLayout.closeDrawer(GravityCompat.START);
                        return true;
                    }

                    else if (itemId == R.id.generateChallan) {
                        tv_title.setText("Generate Challan");
                        add_challan addChallan = new add_challan();
                        FragmentTransaction fragmentTransaction = getSupportFragmentManager().beginTransaction();
                        fragmentTransaction.replace(R.id.Main, addChallan);
                        fragmentTransaction.commit();
                        Toast.makeText(getApplicationContext(), "Generate Challan", Toast.LENGTH_LONG).show();
                        drawerLayout.closeDrawer(GravityCompat.START);
                        return true;
                    }

                    else if (itemId == R.id.generateMChallan) {
                        tv_title.setText("Generate Other Challan");
                        other_fee otherFee = new other_fee();
                        FragmentTransaction fragmentTransaction = getSupportFragmentManager().beginTransaction();
                        fragmentTransaction.replace(R.id.Main, otherFee);
                        fragmentTransaction.commit();
                        Toast.makeText(getApplicationContext(), "Generate M Challan", Toast.LENGTH_LONG).show();
                        drawerLayout.closeDrawer(GravityCompat.START);
                        return true;
                    }

                    else if (itemId == R.id.accountBook) {
                        tv_title.setText("Account Book");
                        Bundle bundle = new Bundle();
                        bundle.putString("Id", user.getNuml_id() );
                        account_book accountBook = new account_book();
                        accountBook.setArguments(bundle);
                        FragmentTransaction fragmentTransaction = getSupportFragmentManager().beginTransaction();
                        fragmentTransaction.replace(R.id.Main, accountBook);
                        fragmentTransaction.commit();
                        Toast.makeText(getApplicationContext(), "Account Book", Toast.LENGTH_LONG).show();
                        drawerLayout.closeDrawer(GravityCompat.START);
                        return true;
                    }

                    else if (itemId == R.id.profile) {
                        tv_title.setText("Profile");
                        profile profile = new profile();
                        FragmentTransaction fragmentTransaction = getSupportFragmentManager().beginTransaction();
                        fragmentTransaction.replace(R.id.Main, profile);
                        fragmentTransaction.commit();
                        Toast.makeText(getApplicationContext(), "Profile", Toast.LENGTH_LONG).show();
                        drawerLayout.closeDrawer(GravityCompat.START);
                        return true;
                    }

                    else if (itemId == R.id.logout) {
                        clearSession();
                        Intent intent = new Intent(MainActivity.this, Auth.class);
                        startActivity(intent);
                        finish();
                        drawerLayout.closeDrawer(GravityCompat.START);
                        return true;
                    }

                    else if (itemId == R.id.exit) {
                        drawerLayout.closeDrawer(GravityCompat.START);
                        finishAffinity();
                        System.exit(0);
                        return true;
                    }

                    return false;
                } else {
                    return false;
                }
            }
        });

    }

    private void clearSession() {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.remove("user"); // Remove the user ID from SharedPreferences
        editor.apply();
    }

    public void ChangeFragment(Fragment fragment){
        FragmentManager fragmentManager = getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.Main, fragment);
        transaction.commit();
    }
}

