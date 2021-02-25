function InternalList(options, model) {
    let $FilterForm = $("#" + options.filterFormId);
    let $table = $("#" + options.tableId);

    this.onFilterCheckChange = (e) => {
        $("#" + $(e.target).attr("id").replace("Filter", ""), $FilterForm).prop("disabled", !e.target.checked);
        this.onFilterChange();
    }

    this.onFilterChange = (e) => {
        console.log(model.dataUrl)
        let dataUrl = model.dataUrl.replace("{filterFromDate}", $FilterForm.find("#FilterFromDate").prop("checked"))
            .replace("{fromDate}", $FilterForm.find("#FromDate").val())
            .replace("{filterUntilDate}", $FilterForm.find("#FilterUntilDate").prop("checked"))
            .replace("{untilDate}", $FilterForm.find("#UntilDate").val())
            .replace("{documentTypeId}", $FilterForm.find("#DocumentTypeId").val())
            .replace("{fromDepartmentId}", $FilterForm.find("#FromDepartmentId").val())
            .replace("{toDepartmentId}", $FilterForm.find("#ToDepartmentId").val())
            .replace("{receiverId}", $FilterForm.find("#ReceiverId").val())
            .replace("{approverId}", $FilterForm.find("#ApproverId").val())
            .replace("{signerId}", $FilterForm.find("#SignerId").val());

        $table.bootstrapTable('refreshOptions', {
            url: dataUrl,
        })
        console.log(dataUrl)
    }

    $("#report-quick-filter").on("change", (e) => {
        let value = $(e.target).val();

        let fromDate = "";
        let untilDate = "";

        switch (value) {
            case "Today": {
                fromDate = moment().format(develoverSettings.formatDate);
                untilDate = fromDate;
            } break;
            case "Yesterday": {
                fromDate = moment().add(-1, "days").format(develoverSettings.formatDate);
                untilDate = fromDate;
            } break;
            case "ThisWeek": {
                fromDate = moment().startOf('isoWeek').format(develoverSettings.formatDate);
                untilDate = moment().endOf('isoWeek').format(develoverSettings.formatDate);
            } break;
            case "ThisMonth": {
                fromDate = moment().month(moment().month()).startOf('month').format(develoverSettings.formatDate);
                untilDate = moment().month(moment().month()).endOf('month').format(develoverSettings.formatDate);
            } break;
            case "ThisQuarter": {
                fromDate = moment().quarter(moment().quarter()).startOf('quarter').format(develoverSettings.formatDate);
                untilDate = moment().quarter(moment().quarter()).endOf('quarter').format(develoverSettings.formatDate);
            } break;
            case "ThisYear": {
                fromDate = moment().year(moment().year()).startOf('year').format(develoverSettings.formatDate);
                untilDate = moment().year(moment().year()).endOf('year').format(develoverSettings.formatDate);
            } break;
        }

        this.removeEventsListener();

        $("#FilterFromDate", $FilterForm).prop("checked", true);
        $("#FromDate", $FilterForm).datepicker("update", fromDate);
        $("#FromDate", $FilterForm).prop("disabled", false);
        $("#FilterUntilDate", $FilterForm).prop("checked", true);
        $("#UntilDate", $FilterForm).datepicker("update", untilDate);
        $("#UntilDate", $FilterForm).prop("disabled", false);

        $("#DocumentTypeId", $FilterForm).val();
        $("#FromDepartmentId", $FilterForm).val();
        $("#ToDepartmentId", $FilterForm).val();
        $("#ReceiverId", $FilterForm).val();
        $("#ApproverId", $FilterForm).val();
        $("#SignerId", $FilterForm).val();
        this.onFilterChange();

        this.addEventsListener();
    });

    $table.on("load-success.bs.table", (data) => {
        $FilterForm.removeClass("disabled");
    });

    $table.on("refresh-options.bs.table", (options) => {
        $FilterForm.addClass("disabled");
    });

    this.addEventsListener = () => {
        $("input[type=\"checkbox\"]", $FilterForm).on("change", this.onFilterCheckChange);
        $("input[type=\"text\"]", $FilterForm).on("change", this.onFilterChange);
        $("select", $FilterForm).on("change", this.onFilterChange);
    }

    this.removeEventsListener = () => {
        $("input[type=\"checkbox\"]", $FilterForm).off("change");
        $("input[type=\"text\"]", $FilterForm).off("change");
        $("select", $FilterForm).off("change");
    }

    this.initialize = () => {
        initFilterDateDefaultValue($FilterForm);
        this.addEventsListener();

        let dataUrl = model.dataUrl.replace("{filterFromDate}", $FilterForm.find("#FilterFromDate").prop("checked"))
            .replace("{fromDate}", $FilterForm.find("#FromDate").val())
            .replace("{filterUntilDate}", $FilterForm.find("#FilterUntilDate").prop("checked"))
            .replace("{untilDate}", $FilterForm.find("#UntilDate").val())
            .replace("{documentTypeId}", $FilterForm.find("#DocumentTypeId").val())
            .replace("{fromDepartmentId}", $FilterForm.find("#FromDepartmentId").val())
            .replace("{toDepartmentId}", $FilterForm.find("#ToDepartmentId").val())
            .replace("{receiverId}", $FilterForm.find("#ReceiverId").val())
            .replace("{approverId}", $FilterForm.find("#ApproverId").val())
            .replace("{signerId}", $FilterForm.find("#SignerId").val());

        initBootstrapTable(options.tableId, model.columns, [], dataUrl);
    }
}