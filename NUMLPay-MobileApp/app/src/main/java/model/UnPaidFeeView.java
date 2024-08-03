package model;

public class UnPaidFeeView {
    private int challan_no;
    private int semester;
    private int fee_type;
    private int fee_for;
    private String FeeFor;
    private String issue_date;
    private String installment_no;
    private float total_fee;
    private String due_date;
    private String status;
    private String valid_date;
    private int id;
    private float fine;
    private String paid_date;
    private String payment_method;

    // Constructor
    public UnPaidFeeView() {
        // Default constructor
    }

    // Getters and setters
    public int getChallan_no() {
        return challan_no;
    }

    public void setChallan_no(int challan_no) {
        this.challan_no = challan_no;
    }

    public int getSemester() {
        return semester;
    }

    public void setSemester(int semester) {
        this.semester = semester;
    }

    public int getFee_type() {
        return fee_type;
    }

    public void setFee_type(int fee_type) {
        this.fee_type = fee_type;
    }

    public int getFee_for() {
        return fee_for;
    }

    public void setFee_for(int fee_for) {
        this.fee_for = fee_for;
    }

    public String getFeeFor() {
        return FeeFor;
    }

    public void setFeeFor(String feeFor) {
        FeeFor = feeFor;
    }

    public String getIssue_date() {
        return issue_date;
    }

    public void setIssue_date(String issue_date) {
        this.issue_date = issue_date;
    }

    public String getInstallment_no() {
        return installment_no;
    }

    public void setInstallment_no(String installment_no) {
        this.installment_no = installment_no;
    }

    public float getTotal_fee() {
        return total_fee;
    }

    public void setTotal_fee(float total_fee) {
        this.total_fee = total_fee;
    }

    public String getDue_date() {
        return due_date;
    }

    public void setDue_date(String due_date) {
        this.due_date = due_date;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getValid_date() {
        return valid_date;
    }

    public void setValid_date(String valid_date) {
        this.valid_date = valid_date;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public float getFine() {
        return fine;
    }

    public void setFine(float fine) {
        this.fine = fine;
    }

    public String getPaid_date() {
        return paid_date;
    }

    public void setPaid_date(String paid_date) {
        this.paid_date = paid_date;
    }

    public String getPayment_method() {
        return payment_method;
    }

    public void setPayment_method(String payment_method) {
        this.payment_method = payment_method;
    }
}

