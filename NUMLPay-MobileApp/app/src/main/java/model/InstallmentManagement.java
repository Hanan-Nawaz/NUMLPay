package model;

public class InstallmentManagement {
    private int installment_id;
    private int mode;
    private int fee_for;
    private int is_active;
    private String added_by;

    // Constructors
    public InstallmentManagement() {
    }

    // Getters and setters
    public int getInstallment_id() {
        return installment_id;
    }

    public void setInstallment_id(int installment_id) {
        this.installment_id = installment_id;
    }

    public int getMode() {
        return mode;
    }

    public void setMode(int mode) {
        this.mode = mode;
    }

    public int getFee_for() {
        return fee_for;
    }

    public void setFee_for(int fee_for) {
        this.fee_for = fee_for;
    }

    public int getIs_active() {
        return is_active;
    }

    public void setIs_active(int is_active) {
        this.is_active = is_active;
    }

    public String getAdded_by() {
        return added_by;
    }

    public void setAdded_by(String added_by) {
        this.added_by = added_by;
    }
}

