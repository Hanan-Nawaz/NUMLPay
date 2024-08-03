package model;

public class SpinnerItem {
    private int id;
    private String text;

    public SpinnerItem(int id, String text) {
        this.id = id;
        this.text = text;
    }

    public int getId() {
        return id;
    }

    public String getText() {
        return text;
    }

    @Override
    public String toString() {
        return text; // Display text in spinner
    }
}

