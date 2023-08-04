package com.assambra.common.mail.mailbodys;

import com.assambra.common.mail.interfaces.MailBody;
import freemarker.template.Configuration;
import freemarker.template.Template;
import freemarker.template.TemplateException;

import java.io.IOException;
import java.io.StringWriter;
import java.util.Map;

public class NewPasswordMailBody implements MailBody {
    private Configuration configuration;
    private Template bodyTemplate;
    private Map<String, Object> dataModel;

    public void setConfiguration(Configuration configuration) {
        this.configuration = configuration;
    }

    public void loadTemplate() throws IOException {
        bodyTemplate = configuration.getTemplate("new_password_body.ftl");
    }

    public void setDataModel(Map<String, Object> dataModel) {
        this.dataModel = dataModel;
    }

    public String buildBody() throws IOException, TemplateException {
        StringWriter bodyWriter = new StringWriter();
        bodyTemplate.process(dataModel, bodyWriter);
        return bodyWriter.toString();
    }
}
