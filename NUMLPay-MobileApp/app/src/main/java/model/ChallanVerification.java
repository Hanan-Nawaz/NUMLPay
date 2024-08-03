package model;

public class ChallanVerification {
    private int id;
    private int fee_installment_id;
    private String image;
    private String verified_by;

    // Constructors
    public ChallanVerification() {
    }

    // Getters and setters
    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getFee_installment_id() {
        return fee_installment_id;
    }

    public void setFee_installment_id(int fee_installment_id) {
        this.fee_installment_id = fee_installment_id;
    }


    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getVerified_by() {
        return verified_by;
    }

    public void setVerified_by(String verified_by) {
        this.verified_by = verified_by;
    }

}
