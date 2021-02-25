var develoverReport = {
    filterFromDate: true,
    fromDate: moment().date(1).format(develoverSettings.formatDate),
    filterUntilDate: true,
    untilDate: moment().date(1).add(1, "months").add(-1, "days").format(develoverSettings.formatDate),
}

var initFilterDateDefaultValue = (context) => {
    $("#FilterFromDate", context).prop("checked", develoverReport.filterFromDate);
    $("#FromDate", context).datepicker("update", develoverReport.fromDate);
    $("#FromDate", context).prop("disabled", !develoverReport.filterFromDate);
    $("#FilterUntilDate", context).prop("checked", develoverReport.filterUntilDate);
    $("#UntilDate", context).datepicker("update", develoverReport.untilDate);
    $("#UntilDate", context).prop("disabled", !develoverReport.filterUntilDate);
}