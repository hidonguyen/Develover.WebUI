function Employee(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $modal = $("#" + options.modalId);

    let postbackId = "";
    let postbackClass = "";

    /** Modal */

    $modal.on("shown.bs.modal", (e) => {
        postbackId = $(e.relatedTarget).data("postback-id");
        postbackClass = $(e.relatedTarget).data("postback-class");

        $form.find("#Id").val(uuidv4());
        $form.find("#PermanentAddressId").val(uuidv4());
        $form.find("#TemporaryAddressId").val(uuidv4());
    })

    $modal.on("hidden.bs.modal", (e) => {
        postbackId = "";
        postbackClass = "";

        $modal.find("input:not(.datepicker), textarea").val("");
        $modal.find("input.datepicker").datepicker("update", "");
        $modal.find("select.selectpicker").selectpicker("val", "");

        $("input#Status", $form).prop("checked", true);
        $("input#Status", $form).val(true);

        $form.find(".is-invalid").removeClass("is-invalid");
        $form.find(".invalid-feedback").html("");
    })

    this.hideModal = () => {
        $modal.modal("hide");
    }
    /** Modal */

    /** Master */

    function createModel() {
        model.code = $("input#Code", $form).val();
        model.refCode = $("input#RefCode", $form).val();

        model.firstName = $("input#FirstName", $form).val();
        model.middleName = $("input#MiddleName", $form).val();
        model.lastName = $("input#LastName", $form).val();
        //
        model.email = $("input#Email", $form).val();
        model.phone = $("input#Phone", $form).val();
        model.alternativePhone = $("input#AlternativePhone", $form).val();
        //
        model.hireDate = $("input#HireDate", $form).val();
        model.originalHireDate = $("input#OriginalHireDate", $form).val();
        model.terminationDate = $("input#TerminationDate", $form).val();
        model.terminationReason = $("input#TerminationReason", $form).val();
        model.reHireDate = $("input#ReHireDate", $form).val();
        //
        model.divisionId = $("select#DivisionId", $form).val();
        model.positionId = $("select#PositionId", $form).val();

        model.note = $("textarea#Note", $form).val();
        model.status = $("input#Status", $form).prop("checked");

        //EmployeeIdentification
        model.identificationId = $("input#IdentificationId", $form).val();
        model.identification.type = $("input#Identification_Type", $form).val();
        model.identification.no = $("input#Identification_No", $form).val();
        model.identification.issueDate = $("input#Identification_IssueDate", $form).val();
        model.identification.issuePlace = $("input#Identification_IssuePlace", $form).val();
        model.identification.expireDate = $("input#Identification_ExpireDate", $form).val();
        model.identification.note = $("textarea#Identification_Note", $form).val();
        //EmployeeBiography
        model.biographyId = $("input#BiographyId", $form).val();
        model.biography.dateOfBirth = $("input#Biography_DateOfBirth", $form).val();
        model.biography.placeOfBirth = $("input#Biography_PlaceOfBirth", $form).val();
        model.biography.genderId = $("select#GenderId", $form).val();
        model.biography.maritalStatusId = $("select#MaritalStatusId", $form).val();
        model.biography.ethnicGroupId = $("select#EthnicGroupId", $form).val();
        //PermanentAddress
        model.permanentAddressId = $("input#PermanentAddressId", $form).val();
        model.permanentAddress.line1 = $("input#PermanentAddress_Line1", $form).val();
        model.permanentAddress.line2 = $("input#PermanentAddress_Line2", $form).val();
        model.permanentAddress.city = $("input#PermanentAddress_City", $form).val();
        model.permanentAddress.country = $("input#PermanentAddress_Country", $form).val();
        model.permanentAddress.note = $("textarea#PermanentAddress_Note", $form).val();
        //TemporaryAddress
        model.temporaryAddressId = $("input#TemporaryAddressId", $form).val();
        model.temporaryAddress.line1 = $("input#TemporaryAddress_Line1", $form).val();
        model.temporaryAddress.line2 = $("input#TemporaryAddress_Line2", $form).val();
        model.temporaryAddress.city = $("input#TemporaryAddress_City", $form).val();
        model.temporaryAddress.country = $("input#TemporaryAddress_Country", $form).val();
        model.temporaryAddress.note = $("textarea#TemporaryAddress_Note", $form).val();
        //
        model.socialInsuranceNo = $("input#SocialInsuranceNo", $form).val();
        model.socialInsuranceDate = $("input#SocialInsuranceDate", $form).val();
        model.healthInsuranceNo = $("input#HealthInsuranceNo", $form).val();
        model.healthInsuranceDate = $("input#HealthInsuranceDate", $form).val();
        //
        model.personalTaxCode = $("input#PersonalTaxCode", $form).val();
        //
        model.dependents = employeeDependentModal.createModel();
    }

    this.save = () => {
        createModel();

        $.ajax({
            url: "/catalog/saveemployee",
            type: "post",
            data: { mode, model }
        }).done((res) => {
            swal({
                title: "Success",
                text: "The Employee has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                if (options.modalId && postbackId && postbackId !== "" && postbackClass && postbackClass !== "") {
                    if (model.status) {
                        $("select." + postbackClass).map((index, select) => {
                            $(select).append("<option value=\"" + res.id + "\">" + res.name + "</option>");
                            $(select).selectpicker("refresh");
                        });

                        $("#" + postbackId).selectpicker("val", res.id);
                        $("button[data-id=\"" + postbackId + "\"]").attr("title", res.name);
                        $(".filter-option-inner-inner", $("button[data-id=\"" + postbackId + "\"]")).text(res.name);
                    }

                    this.hideModal();
                }
                else {
                    window.location.href = "/catalog/detailemployee?id=" + res.id;
                }
            })
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
        });
    }

    this.delete = () => {
        swal({
            title: "Delete Employee?",
            text: "Do you want to delete the employee now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/catalog/deleteemployee",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The employee has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/catalog/employees";
                    })
                }).fail((err) => {
                    swal("[" + err.status + "] " + err.responseText, {
                        icon: "error",
                    });
                });
            }
        });
    }


    /** Master */


    this.initialize = () => {
        if (mode === CatalogMode.VIEW) {
            $form.find("input:not(.datepicker), textarea").prop("readonly", true);
            $form.find("input.datepicker, select, button, a, input[type=\"checkbox\"]").prop("disabled", true);
            $form.find(".add-employee-dependent").remove();
        }
    }

}

function EmployeeDependent(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $modal = $("#" + options.modalId);
    let $table = $("#" + options.tableId);

    /** Modal */

    window.operateEvents = {
        "click .edit": function (e, value, row, index) {
            $modal.data("rowData", row);
            $modal.data("rowIndex", index);
            $modal.modal("show");
        },
        "click .delete": function (e, value, row, index) {
            swal({
                title: "Delete row?",
                text: "Do you want to delete this row now? This cannot be undone.",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            }).then((res) => {
                if (res) {
                    $table.bootstrapTable("remove", {
                        field: "id",
                        values: [row.id]
                    });
                }
            });
        }
    }

    $modal.on("hidden.bs.modal", (e) => {
        $modal.find("input:not(.datepicker), textarea").val("");
        $modal.find("input.datepicker").datepicker("update", "");
        $modal.find("select.selectpicker").selectpicker("val", "");

        $form.find(".is-invalid").removeClass("is-invalid");
        $form.find(".invalid-feedback").html("");

        $modal.removeData("rowIndex");
        $modal.removeData("rowData");
    })

    $modal.on("show.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modal.find("h5.modal-title").text("Add dependent");
            $modal.find("button.save").text("Add");
        }
        else {
            $modal.find("h5.modal-title").text("Edit dependent");
            $modal.find("button.save").text("Save");
        }
    })

    $modal.on("shown.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData) {
            $("input#Id", $form).val(rowData.id);
            $("input#FirstName", $form).val(rowData.firstName);
            $("input#MiddleName", $form).val(rowData.middleName);
            $("input#LastName", $form).val(rowData.lastName);
            $("input#Relationship", $form).val(rowData.relationship);

            //EmployeeBiography
            $("input#DateOfBirth", $form).datepicker("update", rowData.dateOfBirth);
            $("input#PlaceOfBirth", $form).val(rowData.placeOfBirth);
            $("select#GenderId", $form).selectpicker('val', rowData.genderId);
            $("select#MaritalStatusId", $form).selectpicker('val', rowData.maritalStatusId);
            $("select#EthnicGroupId", $form).selectpicker('val', rowData.ethnicGroupId);
            //PermanentAddress
            $("input#PermanentAddressId", $form).val(rowData.permanentAddressId);
            $("input#PermanentAddress_Line1", $form).val(rowData.permanentAddressLine1);
            $("input#PermanentAddress_Line2", $form).val(rowData.permanentAddressLine2);
            $("input#PermanentAddress_City", $form).val(rowData.permanentAddressCity);
            $("input#PermanentAddress_Country", $form).val(rowData.permanentAddressCountry);
            $("textarea#PermanentAddress_Note", $form).val(rowData.permanentAddressNote);
            //TemporaryAddress
            $("input#TemporaryAddressId", $form).val(rowData.temporaryAddressId);
            $("input#TemporaryAddress_Line1", $form).val(rowData.temporaryAddressLine1);
            $("input#TemporaryAddress_Line2", $form).val(rowData.temporaryAddressLine2);
            $("input#TemporaryAddress_City", $form).val(rowData.temporaryAddressCity);
            $("input#TemporaryAddress_Country", $form).val(rowData.temporaryAddressCountry);
            $("textarea#TemporaryAddress_Note", $form).val(rowData.temporaryAddressNote);
            //
            $("input#Email", $form).val(rowData.email);
            $("input#Phone", $form).val(rowData.phone);
            $("input#AlternativePhone", $form).val(rowData.alternativePhone);

            $("textarea#Note", $form).val(rowData.note);
        }
    })

    this.hideModal = () => {
        $modal.modal("hide");
    }

    this.createModel = () => {
        let dependents = [];

        $table.bootstrapTable('getData').map((rowData, index) => {
            let dependent = {
                id: rowData.id,
                sequenceNo: index + 1,
                firstName: rowData.firstName,
                middleName: rowData.middleName,
                lastName: rowData.lastName,
                relationship: rowData.relationship,
                //EmployeeBiography
                dateOfBirth: rowData.dateOfBirth,
                placeOfBirth: rowData.placeOfBirth,
                genderId: rowData.genderId,
                maritalStatusId: rowData.maritalStatusId,
                ethnicGroupId: rowData.ethnicGroupId,
                //PermanentAddress
                permanentAddressId: rowData.permanentAddressId,
                permanentAddress: {
                    line1: rowData.permanentAddressLine1,
                    line2: rowData.permanentAddressLine2,
                    city: rowData.permanentAddressCity,
                    country: rowData.permanentAddressCountry,
                    note: rowData.permanentAddressNote,
                },
                //TemporaryAddress
                temporaryAddressId: rowData.temporaryAddressId,
                temporaryAddress: {
                    line1: rowData.temporaryAddressLine1,
                    line2: rowData.temporaryAddressLine2,
                    city: rowData.temporaryAddressCity,
                    country: rowData.temporaryAddressCountry,
                    note: rowData.temporaryAddressNote,
                },
                //
                email: rowData.email,
                phone: rowData.phone,
                alternativePhone: rowData.alternativePhone,

                note: rowData.note,
            };            

            dependents.push(dependent);
        });

        return dependents;
    }

    this.addDependent = () => {
        let index = $modal.data("rowIndex");
        let boostrapTableMethod = "updateRow";
        if (index === undefined) {
            index = $table.bootstrapTable("getData").length - 1; //Trừ đi để đồng nhất với rowIndex => sequenceNo sẽ + 1
            boostrapTableMethod = "insertRow";
        }

        let dependent = {
            id: $("input#Id", $form).val(),
            sequenceNo: index + 1,
            firstName: $("input#FirstName", $form).val(),
            middleName: $("input#MiddleName", $form).val(),
            lastName: $("input#LastName", $form).val(),
            relationship: $("input#Relationship", $form).val(),
            //EmployeeBiography
            dateOfBirth: $("input#DateOfBirth", $form).val(),
            placeOfBirth: $("input#PlaceOfBirth", $form).val(),
            genderId: $("select#GenderId", $form).val(),
            gender: {
                name: $("select#GenderId option:selected", $form).text()
            },
            maritalStatusId: $("select#MaritalStatusId", $form).val(),
            maritalStatus: {
                name: $("select#MaritalStatusId option:selected", $form).text()
            },
            ethnicGroupId: $("select#EthnicGroupId", $form).val(),
            ethnicGroup: {
                name: $("select#EthnicGroupId option:selected", $form).text()
            },
            //PermanentAddress
            permanentAddressId: $("input#PermanentAddressId", $form).val(),
            permanentAddress: {
                line1: $("input#PermanentAddress_Line1", $form).val(),
                line2: $("input#PermanentAddress_Line2", $form).val(),
                city: $("input#PermanentAddress_City", $form).val(),
                country: $("input#PermanentAddress_Country", $form).val(),
                note: $("textarea#PermanentAddress_Note", $form).val(),
            },
            //TemporaryAddress
            temporaryAddressId: $("input#TemporaryAddressId", $form).val(),
            temporaryAddress: {
                line1: $("input#TemporaryAddress_Line1", $form).val(),
                line2: $("input#TemporaryAddress_Line2", $form).val(),
                city: $("input#TemporaryAddress_City", $form).val(),
                country: $("input#TemporaryAddress_Country", $form).val(),
                note: $("textarea#TemporaryAddress_Note", $form).val(),
            },
            //
            email: $("input#Email", $form).val(),
            phone: $("input#Phone", $form).val(),
            alternativePhone: $("input#AlternativePhone", $form).val(),

            note: $("textarea#Note", $form).val(),
        };

        $.ajax({
            url: "/catalog/validateemployeedependentmodel",
            type: "post",
            data: { model: dependent }
        }).done((res) => {
            $table.bootstrapTable(boostrapTableMethod, {
                index: index,
                row: res
            });
            $modal.modal("hide");
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
        });
    }

    this.initialize = () => {
        let columns = model.detailDependentColumns;
        if (mode === CatalogMode.VIEW) {
            columns.splice(1, 1); // remove operate column
        }
        initBootstrapTable(options.tableId, columns, [], model.detailDependentDataUrl);
    }
}
