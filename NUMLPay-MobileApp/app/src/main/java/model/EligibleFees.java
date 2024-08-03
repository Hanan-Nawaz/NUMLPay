package model;

public class EligibleFees {
    private int id;
    private String std_numl_id;
    private Users users;
    private int semester_fee;
    private int hostel_fee;
    private int bus_fee;

    // Constructors
    public EligibleFees() {
    }

    public EligibleFees(int id, String std_numl_id, Users users, int semester_fee, int hostel_fee, int bus_fee) {
        this.id = id;
        this.std_numl_id = std_numl_id;
        this.users = users;
        this.semester_fee = semester_fee;
        this.hostel_fee = hostel_fee;
        this.bus_fee = bus_fee;
    }

    // Getters and setters
    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getStd_numl_id() {
        return std_numl_id;
    }

    public void setStd_numl_id(String std_numl_id) {
        this.std_numl_id = std_numl_id;
    }

    public Users getUsers() {
        return users;
    }

    public void setUsers(Users users) {
        this.users = users;
    }

    public int getSemester_fee() {
        return semester_fee;
    }

    public void setSemester_fee(int semester_fee) {
        this.semester_fee = semester_fee;
    }

    public int getHostel_fee() {
        return hostel_fee;
    }

    public void setHostel_fee(int hostel_fee) {
        this.hostel_fee = hostel_fee;
    }

    public int getBus_fee() {
        return bus_fee;
    }

    public void setBus_fee(int bus_fee) {
        this.bus_fee = bus_fee;
    }
}
