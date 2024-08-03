package model;

public class EmailModel {
    private String receiverEmail;
    private String subject;
    private String body;
    private String attachmentPath;

    public EmailModel() {
        // Default constructor required for Firebase
    }

    public EmailModel(String receiverEmail, String subject, String body, String attachmentPath) {
        this.receiverEmail = receiverEmail;
        this.subject = subject;
        this.body = body;
        this.attachmentPath = attachmentPath;
    }

    public String getReceiverEmail() {
        return receiverEmail;
    }

    public void setReceiverEmail(String receiverEmail) {
        this.receiverEmail = receiverEmail;
    }

    public String getSubject() {
        return subject;
    }

    public void setSubject(String subject) {
        this.subject = subject;
    }

    public String getBody() {
        return body;
    }

    public void setBody(String body) {
        this.body = body;
    }

    public String getAttachmentPath() {
        return attachmentPath;
    }

    public void setAttachmentPath(String attachmentPath) {
        this.attachmentPath = attachmentPath;
    }
}

