function logAjaxFail(where, jqXHR) {
    // No-op
}

$(document).on("click", ".cmd-report-bug, .cmd-report-need", function () {
    function _getProjectEpic() {
        var path = window.location.pathname;
        var pathParts = path.split("/");
        var page = pathParts[pathParts.length - 1];

        if (page in JIRA.project) {
            return JIRA.project[page];
        } else {
            return JIRA.default_project;
        }
    }

    function _isCompatible() {
        var ua = navigator.userAgent;
        return ua.indexOf('Trident') === -1 && (ua.indexOf("NT 6.1") === -1 || ua.indexOf("NT 6.3") === -1);
    }

    function _openInChrome(target) {
        if ("ActiveXObject" in window) {
            var oShell = new ActiveXObject('Shell.Application');
            oShell.ShellExecute('chrome.exe', target, '', '', '1');
        }
    }
    // Some constants

    var JIRA = {
        script_url: 'https://jira.tools.tax.service.gov.uk/s/2efa2d2ce2ded111a1eb0db9ca3946ac-T/en_UKtc5hgs/71014/be09033ea7858348cd8d5cdeb793189a/2.0.14/_/download/batch/com.atlassian.jira.collector.plugin.jira-issue-collector-plugin:issuecollector/com.atlassian.jira.collector.plugin.jira-issue-collector-plugin:issuecollector.js?locale=en-UK&collectorId=',

        project: {
            "howtoEditor.htm": "60",
            "processEditor.htm": "16",
            "process.htm": "16"
        },

        default_project: "16",

        ids: {
            bug: "75b5b0a6",
            need: "addd64ae"
        }
    };

    var id = this.classList.contains("cmd-report-bug") ? JIRA.ids.bug : JIRA.ids.need;
    var $this = $(this);
    var project = _getProjectEpic();

    if (!_isCompatible()) {
        _openInChrome('http://bg.cc.inrev.gov.uk/cms/admin/JiraCollector.htm?id=' + id + '&gd=' + project);
        return;
    }

    // Set JIRA properties

    ATL_JQ_PAGE_PROPS = {

        // This comes from JIRA, and I feel it should be called "onload" since
        // it's run when the script loads. Ah, well.

        'triggerFunction': function (dialog) {
            $this.on("click", dialog);
            setTimeout(dialog, 25);
        },

        'fieldValues': {
            // Set the epic for this issue.
            customfield_10008: 'GD-' + project
        }
    };

    // Disable on click while the script is loading
    $this.off("click");

    // Load and run the script from JIRA
    $.getScript(JIRA.script_url + id);
});