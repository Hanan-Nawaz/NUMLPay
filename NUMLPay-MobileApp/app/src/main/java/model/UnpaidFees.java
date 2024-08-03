package model;

public class UnpaidFees {
    private int challan_no;
    private String std_numl_id;
    private int fee_type;
    private int semester;
    private String issue_date;
    private int security;
    private int fee_id;

    public UnpaidFees(int challan_no, String std_numl_id, int fee_type, int semester, String issue_date, int security, int fee_id) {
        this.challan_no = challan_no;
        this.std_numl_id = std_numl_id;
        this.fee_type = fee_type;
        this.semester = semester;
        this.issue_date = issue_date;
        this.security = security;
        this.fee_id = fee_id;
    }

    public UnpaidFees() {

    }

    public int getChallan_no() {
        return challan_no;
    }

    public void setChallan_no(int challan_no) {
        this.challan_no = challan_no;
    }

    public int getFee_id() {
        return fee_id;
    }

    public void setFee_id(int fee_id) {
        this.fee_id = fee_id;
    }

    public String getStd_numl_id() {
        return std_numl_id;
    }

    public void setStd_numl_id(String std_numl_id) {
        this.std_numl_id = std_numl_id;
    }

    public int getFee_type() {
        return fee_type;
    }

    public void setFee_type(int fee_type) {
        this.fee_type = fee_type;
    }

    public int getSemester() {
        return semester;
    }

    public void setSemester(int semester) {
        this.semester = semester;
    }

    public String getIssue_date() {
        return issue_date;
    }

    public void setIssue_date(String issue_date) {
        this.issue_date = issue_date;
    }

    public int getSecurity() {
        return security;
    }

    public void setSecurity(int security) {
        this.security = security;
    }
}
