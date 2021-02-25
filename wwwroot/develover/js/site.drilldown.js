
function branchDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/branches/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function userDrillDownFormatter(data, type, row, meta) {
    if (row.userId && data != undefined) {
        return '<a href="/identity/detailuser?id=' + row.userId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/identity/detailuser?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function roleDrillDownFormatter(data, type, row, meta) {
    if (row.roleId && data != undefined) {
        return '<a href="/identity/detailrole?id=' + row.roleId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/identity/detailrole?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function permissionDrillDownFormatter(data, type, row, meta) {
    if (row.permissionId && data != undefined) {
        return '<a href="/identity/detailpermission?id=' + row.permissionId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/identity/detailpermission?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function currencyDrillDownFormatter(data, type, row, meta) {
    if (row.currencyId && data != undefined) {
        return '<a href="/identity/detailcurrency?id=' + row.currencyId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/identity/detailcurrency?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}

function baseCatalogDrillDownFormatter(data, type, row, meta) {
    if (row.baseCatalogId && data != undefined) {
        return '<a href="/catalog/detailbaseCatalog?id=' + row.baseCatalogId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailbaseCatalog?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function departmentDrillDownFormatter(data, type, row, meta) {
    if (row.departmentId && data != undefined) {
        return '<a href="/catalog/detaildepartment?id=' + row.departmentId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detaildepartment?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function documentCategoryDrillDownFormatter(data, type, row, meta) {
    if (row.documentCategoryId && data != undefined) {
        return '<a href="/catalog/detaildocumentCategory?id=' + row.documentCategoryId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detaildocumentCategory?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function documentStatusDrillDownFormatter(data, type, row, meta) {
    if (row.documentStatusId && data != undefined) {
        return '<a href="/catalog/detaildocumentstatus?id=' + row.documentStatusId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detaildocumentstatus?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function documentTypeDrillDownFormatter(data, type, row, meta) {
    if (row.documentTypeId && data != undefined) {
        return '<a href="/catalog/detaildoccumenttype?id=' + row.documentTypeId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detaildocumenttype?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function employeeDrillDownFormatter(data, type, row, meta) {
    if (row.employeeId && data != undefined) {
        return '<a href="/catalog/detailemployee?id=' + row.employeeId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailemployee?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function publishedPlaceDrillDownFormatter(data, type, row, meta) {
    if (row.publishedPlaceId && data != undefined) {
        return '<a href="/catalog/detailpublishedPlace?id=' + row.publishedPlaceId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailpublishedPlace?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function baseVoucherDrillDownFormatter(data, type, row, meta) {
    if (row.baseVoucherId && data != undefined) {
        return '<a href="/basevoucher/detail?id=' + row.baseVoucherId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/basevoucher/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}


function divisionDrillDownFormatter(data, type, row, meta) {
    if (row.divisionId && data != undefined) {
        return '<a href="/catalog/detaildivision?id=' + row.divisionId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detaildivision?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}

function holidayDrillDownFormatter(data, type, row, meta) {
    if (row.holidayId && data != undefined) {
        return '<a href="/catalog/detailholiday?id=' + row.holidayId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailholiday?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function leaveTypeDrillDownFormatter(data, type, row, meta) {
    if (row.leaveTypeId && data != undefined) {
        return '<a href="/catalog/detailleaveType?id=' + row.leaveTypeId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailleaveType?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function positionDrillDownFormatter(data, type, row, meta) {
    if (row.positionId && data != undefined) {
        return '<a href="/catalog/detailposition?id=' + row.positionId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailposition?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function contractTypeDrillDownFormatter(data, type, row, meta) {
    if (row.contractTypeId && data != undefined) {
        return '<a href="/catalog/detailcontractType?id=' + row.contractTypeId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailcontractType?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}

function inboxDocumentDrillDownFormatter(data, type, row, meta) {
    if (row.inboxDocumentId && data != undefined) {
        return '<a href="/documents/inbox/detail?id=' + row.inboxDocumentId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/documents/inbox/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function outboxDocumentDrillDownFormatter(data, type, row, meta) {
    if (row.outboxDocumentId && data != undefined) {
        return '<a href="/documents/outbox/detail?id=' + row.outboxDocumentId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/documents/outbox/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function internalDocumentDrillDownFormatter(data, type, row, meta) {
    if (row.internalDocumentId && data != undefined) {
        return '<a href="/documents/internal/detail?id=' + row.internalDocumentId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/documents/internal/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}

function stockItemDrillDownFormatter(data, type, row, meta) {
    if (row.stockItemId && data != undefined) {
        return '<a href="/catalog/detailstockitem?id=' + row.stockItemId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailstockitem?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function locationDrillDownFormatter(data, type, row, meta) {
    if (row.locationId && data != undefined) {
        return '<a href="/catalog/detaillocation?id=' + row.locationId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detaillocation?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function unitOfMeasureDrillDownFormatter(data, type, row, meta) {
    if (row.unitOfMeasureId && data != undefined) {
        return '<a href="/catalog/detailunitOfMeasure?id=' + row.unitOfMeasureId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailunitOfMeasure?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function supplierDrillDownFormatter(data, type, row, meta) {
    if (row.supplierId && data != undefined) {
        return '<a href="/catalog/detailsupplier?id=' + row.supplierId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailsupplier?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function goodsReceiptNoteDrillDownFormatter(data, type, row, meta) {
    if (row.goodsReceiptNoteId && data != undefined) {
        return '<a href="/stationery/receipt-note/detail?id=' + row.goodsReceiptNoteId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/stationery/receipt-note/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}

function openingStockDrillDownFormatter(data, type, row, meta, index) {
    let dateFormat = ['YYYY-MM-DDThh:mm:ss'];
    let val = "";
    if (moment(data, dateFormat).isValid()) {
        if (moment(data, dateFormat).year === 1900)
            val = "";
        else
            val = moment(data, dateFormat).format(develoverSettings.formatDate);
    }
    else
        val = data;

    if (row.openingStockId && data != undefined) {
        return '<a href="/stationery/openingstock/detail?id=' + row.openingStockId.toLowerCase() + '">' + val + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/stationery/openingstock/detail?id=' + row.id.toLowerCase() + '">' + val + '</a>';
        }
        else {
            return val;
        }
    }
}

function goodsIssueNoteDrillDownFormatter(data, type, row, meta) {
    if (row.goodsIssueNoteId && data != undefined) {
        return '<a href="/stationery/issue-note/detail?id=' + row.goodsIssueNoteId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/stationery/issue-note/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function vehicleFuelingDrillDownFormatter(data, type, row, meta) {
    if (row.vehicleFuelingId && data != undefined) {
        return '<a href="/vehicle/fueling/detail?id=' + row.vehicleFuelingId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/vehicle/fueling/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function vehicleRepairDrillDownFormatter(data, type, row, meta) {
    if (row.vehicleRepairId && data != undefined) {
        return '<a href="/vehicle/repair/detail?id=' + row.vehicleRepairId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/vehicle/repair/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function vehicleScheduleDrillDownFormatter(data, type, row, meta) {
    if (row.vehicleScheduleId && data != undefined) {
        return '<a href="/vehicle/schedule/detail?id=' + row.vehicleScheduleId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/vehicle/schedule/detail?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}

function vehicleUsePurposeDrillDownFormatter(data, type, row, meta) {
    if (row.vehicleUsePurposeId && data != undefined) {
        return '<a href="/catalog/detailVehicleUsePurpose?id=' + row.vehicleUsePurposeId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailVehicleUsePurpose?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function vehicleCostDrillDownFormatter(data, type, row, meta) {
    if (row.vehicleCostId && data != undefined) {
        return '<a href="/catalog/detailVehicleCost?id=' + row.vehicleCostId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailVehicleCost?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}
function vehicleDrillDownFormatter(data, type, row, meta) {
    if (row.vehicleId && data != undefined) {
        return '<a href="/catalog/detailVehicle?id=' + row.vehicleId.toLowerCase() + '">' + data + '</a>';
    } else {
        if (row.id && data != undefined) {
            return '<a href="/catalog/detailVehicle?id=' + row.id.toLowerCase() + '">' + data + '</a>';
        }
        else {
            return data;
        }
    }
}