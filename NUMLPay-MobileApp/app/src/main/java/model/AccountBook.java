package model;

import androidx.annotation.NonNull;

public class AccountBook {
    private int account_id;
    private String std_numl_id;
    private int challan_no;

    public int getAccount_id() {
        return account_id;
    }

    public void setAccount_id(int account_id) {
        this.account_id = account_id;
    }

    @NonNull
    public String getStd_numl_id() {
        return std_numl_id;
    }

    public void setStd_numl_id(@NonNull String std_numl_id) {
        this.std_numl_id = std_numl_id;
    }

    public int getChallan_no() {
        return challan_no;
    }

    public void setChallan_no(int challan_no) {
        this.challan_no = challan_no;
    }

}
