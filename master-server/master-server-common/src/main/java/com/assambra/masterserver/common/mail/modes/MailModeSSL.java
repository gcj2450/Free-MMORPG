package com.assambra.masterserver.common.mail.modes;

import com.assambra.masterserver.common.mail.interfaces.MailMode;
import com.tvd12.ezyfox.util.EzyLoggable;

import javax.mail.Authenticator;
import javax.mail.PasswordAuthentication;
import javax.mail.Session;
import java.util.Properties;

public class MailModeSSL extends EzyLoggable implements MailMode
{
    private final String host;
    private final String port;
    private final String username;
    private final String password;

    public MailModeSSL(String host,String port,String username, String password)
    {
        this.host = host;
        this.port = port;
        this.username = username;
        this.password = password;
    }

    public Session sendMail()
    {
        logger.info("Start SSL mailmode");

        Properties mailprops = new Properties();
        mailprops.put("mail.smtp.host", host);
        mailprops.put("mail.smtp.socketFactory.port", port);
        mailprops.put("mail.smtp.socketFactory.class",
                "javax.net.ssl.SSLSocketFactory");
        mailprops.put("mail.smtp.auth", "true");
        mailprops.put("mail.smtp.port", port);

        Authenticator auth = new Authenticator() {
            protected PasswordAuthentication getPasswordAuthentication() {
                return new PasswordAuthentication(username, password);
            }
        };

        return Session.getInstance(mailprops, auth);
    }
}
