package model;

public class JoinedDataChallan {
    private int challan_no;
    private String issue_date;
    private int installment_no;
    private float fine;
    private float total_fee;
    private String valid_date;
    private String due_date;
    private String paid_date;
    private String Session;
    private int currentSem;
    private int feeSem;
    private String numl_id;
    private String name;
    private String father_name;
    private String fee_plan;
    private String degree_name;
    private int fee_id;
    private Integer fee_for;
    private String FeeFor;
    private String image;
    private String email;

    // Constructors
    public JoinedDataChallan() {
    }

    // Getters and setters
    public int getChallan_no() {
        return challan_no;
    }

    public void setChallan_no(int challan_no) {
        this.challan_no = challan_no;
    }

    public String getIssue_date() {
        return issue_date;
    }

    public void setIssue_date(String issue_date) {
        this.issue_date = issue_date;
    }

    public int getInstallment_no() {
        return installment_no;
    }

    public void setInstallment_no(int installment_no) {
        this.installment_no = installment_no;
    }

    public float getFine() {
        return fine;
    }

    public void setFine(float fine) {
        this.fine = fine;
    }

    public float getTotal_fee() {
        return total_fee;
    }

    public void setTotal_fee(float total_fee) {
        this.total_fee = total_fee;
    }

    public String getValid_date() {
        return valid_date;
    }

    public void setValid_date(String valid_date) {
        this.valid_date = valid_date;
    }

    public String getDue_date() {
        return due_date;
    }

    public void setDue_date(String due_date) {
        this.due_date = due_date;
    }

    public String getPaid_date() {
        return paid_date;
    }

    public void setPaid_date(String paid_date) {
        this.paid_date = paid_date;
    }

    public String getSession() {
        return Session;
    }

    public void setSession(String session) {
        Session = session;
    }

    public int getCurrentSem() {
        return currentSem;
    }

    public void setCurrentSem(int currentSem) {
        this.currentSem = currentSem;
    }

    public int getFeeSem() {
        return feeSem;
    }

    public void setFeeSem(int feeSem) {
        this.feeSem = feeSem;
    }

    public String getNuml_id() {
        return numl_id;
    }

    public void setNuml_id(String numl_id) {
        this.numl_id = numl_id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getFather_name() {
        return father_name;
    }

    public void setFather_name(String father_name) {
        this.father_name = father_name;
    }

    public String getFee_plan() {
        return fee_plan;
    }

    public void setFee_plan(String fee_plan) {
        this.fee_plan = fee_plan;
    }

    public String getDegree_name() {
        return degree_name;
    }

    public void setDegree_name(String degree_name) {
        this.degree_name = degree_name;
    }

    public int getFee_id() {
        return fee_id;
    }

    public void setFee_id(int fee_id) {
        this.fee_id = fee_id;
    }

    public Integer getFee_for() {
        return fee_for;
    }

    public void setFee_for(Integer fee_for) {
        this.fee_for = fee_for;
    }

    public String getFeeFor() {
        return FeeFor;
    }

    public void setFeeFor(String feeFor) {
        FeeFor = feeFor;
    }

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}
