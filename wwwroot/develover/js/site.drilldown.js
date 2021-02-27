
function branchDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/branches/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function contractTypeDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/contracttype/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function departmentDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/department/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function divisionDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/division/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function documentCategoryDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/documentcategory/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function documentStatusDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/documentstatus/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function documentTypeDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/documenttype/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function leaveTypeDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/leavetype/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function locationDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/location/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function positionDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/position/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function publishedPlaceDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/publishedplace/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function unitOfMeasureDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/unitofmeasure/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function vehicleCostDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/vehiclecost/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function vehicleUsePurposeDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/vehicleusepurpose/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function holidayDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/holiday/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function stockItemDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/stockitem/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function supplierDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/supplier/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function vehicleDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/vehicle/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function employeeDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/employee/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function currencyDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/currency/detail?id=' + row.id + '">' + data + '</a>';
    }
    else {
        return data;
    }
}

function permissionDrillDownFormatter(data, type, row, meta) {
    if (row.id && data) {
        return '<a href="/permission/detail?id=' + row.id + '">' + data + '</a>';
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
