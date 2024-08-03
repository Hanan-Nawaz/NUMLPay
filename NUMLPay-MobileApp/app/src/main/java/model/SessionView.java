package model;

public class SessionView {
    private int id;
    private String session;

    // Constructors
    public SessionView() {
    }

    public SessionView(int id, String session) {
        this.id = id;
        this.session = session;
    }

    // Getters and setters
    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getSession() {
        return session;
    }

    public void setSession(String session) {
        this.session = session;
    }
}
