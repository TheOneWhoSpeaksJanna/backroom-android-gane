using System;
using UnityEngine;
using TMPro;

public class DesolationFeedbackController : MonoBehaviour
{
    public TMP_InputField feedbackInput;
    public TMP_InputField emailInput;
    public TMP_Dropdown issueTypeDropdown;

    public void SendFeedback()
    {
        string feedback = feedbackInput != null ? feedbackInput.text : "";
        string email = emailInput != null ? emailInput.text : "";
        string issueType = "General";

        if (issueTypeDropdown != null && issueTypeDropdown.options.Count > 0)
        {
            issueType = issueTypeDropdown.options[issueTypeDropdown.value].text;
        }

        if (string.IsNullOrWhiteSpace(feedback))
        {
            Debug.LogWarning("Feedback is empty.");
            return;
        }

        Debug.Log($"Feedback saved locally. Type: {issueType}, Email: {email}, Message: {feedback}");

        PlayerPrefs.SetString("LastFeedbackMessage", feedback);
        PlayerPrefs.SetString("LastFeedbackEmail", email);
        PlayerPrefs.SetString("LastFeedbackIssueType", issueType);

        if (feedbackInput != null)
        {
            feedbackInput.text = "";
        }
    }
}
