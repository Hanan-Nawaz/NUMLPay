package model;

import java.util.Date;

import javax.persistence.*;

@Entity
@Table(name = "Users")
public class Users {

    @Id
    @Column(name = "numl_id", length = 30)
    private String numl_id;

    @Column(name = "name", length = 50)
    private String name;

    @Column(name = "password", length = 1000)
    private String password;

    @Column(name = "father_name", length = 50)
    private String father_name;

    @Column(name = "date_of_birth")
    private String date_of_birth;

    @Column(name = "email", length = 50)
    private String email;

    @Column(name = "contact", length = 50)
    private String contact;

    @Column(name = "nic", length = 15)
    private String nic;

    private int degree_id;

    @Column(name = "semester")
    private int semester;

    @Column(name = "admission_session")
    private int admission_session;


    private int dept_id;

    @Column(name = "fee_plan")
    private int fee_plan;

    @Column(name = "image", length = 8000)
    private String image;

    @ManyToOne
    @JoinColumn(name = "verified_by")
    private String admin;

    @Column(name = "status_of_degree")
    private int status_of_degree;

    @Column(name = "is_active")
    private int is_active;

    // Getters and setters

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

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getFather_name() {
        return father_name;
    }

    public void setFather_name(String father_name) {
        this.father_name = father_name;
    }

    public String getDate_of_birth() {
        return date_of_birth;
    }

    public void setDate_of_birth(String date_of_birth) {
        this.date_of_birth = date_of_birth;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getContact() {
        return contact;
    }

    public void setContact(String contact) {
        this.contact = contact;
    }

    public String getNic() {
        return nic;
    }

    public void setNic(String nic) {
        this.nic = nic;
    }

    public int getShift() {
        return degree_id;
    }

    public void setShift(int shift) {
        this.degree_id = shift;
    }

    public int getSemester() {
        return semester;
    }

    public void setSemester(int semester) {
        this.semester = semester;
    }

    public int getAdmission_session() {
        return admission_session;
    }

    public void setAdmission_session(int admission_session) {
        this.admission_session = admission_session;
    }

    public int getDepartment() {
        return dept_id;
    }

    public void setDepartment(int department) {
        this.dept_id = department;
    }

    public int getFee_plan() {
        return fee_plan;
    }

    public void setFee_plan(int fee_plan) {
        this.fee_plan = fee_plan;
    }

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getAdmin() {
        return admin;
    }

    public void setAdmin(String admin) {
        this.admin = admin;
    }

    public int getStatus_of_degree() {
        return status_of_degree;
    }

    public void setStatus_of_degree(int status_of_degree) {
        this.status_of_degree = status_of_degree;
    }

    public int getIs_active() {
        return is_active;
    }

    public void setIs_active(int is_active) {
        this.is_active = is_active;
    }
}

