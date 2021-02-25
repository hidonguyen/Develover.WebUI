let decimalSymbol = ".";
let decimalCharacterAlternative = ".";
let thousandsSymbol = ",";
let roundPacking = 0;
let roundQuantity = 2;
let roundAmount = 2;
let roundAmountVND = 0;
let formatDate = "DD/MM/YYYY";
let formatDateDatepicker = "dd/mm/yyyy";
let swalOptions = {
    timer: 1000,
    closeOnClickOutside: true,
    closeOnEsc: true
};

$.ajax({
    url: "/setting/getdeveloversetting",
    type: "get",
    async: false,
}).done((res) => {
    if (res.develoverSetting) {
        decimalSymbol = res.develoverSetting.decimalSymbol === "-1" ? "." : res.develoverSetting.decimalSymbol;
        decimalCharacterAlternative = res.develoverSetting.decimalSymbol === "-1" ? " " : res.develoverSetting.decimalSymbol;
        thousandsSymbol = res.develoverSetting.thousandsSymbol === "-1" ? " " : res.develoverSetting.thousandsSymbol;
        roundPacking = res.develoverSetting.roundPacking;
        roundQuantity = res.develoverSetting.roundQuantity;
        roundAmount = res.develoverSetting.roundAmount;
        roundAmountVND = res.develoverSetting.roundAmountVND;
        formatDate = res.develoverSetting.formatDate;
        formatDateDatepicker = res.develoverSetting.formatDateDatepicker;
    }

}).fail((err) => {
    console.log(err);
});


const develoverSettings = {
    decimalSymbol: decimalSymbol,
    thousandsSymbol: thousandsSymbol,
    roundPacking: roundPacking,
    roundQuantity: roundQuantity,
    roundAmount: roundAmount,
    roundAmountVND: roundAmountVND,
    formatDate: formatDate,
    formatDateDatepicker: formatDateDatepicker,
    OptionAutoNumericPacking: {
        decimalCharacter: decimalSymbol,
        decimalCharacterAlternative: decimalCharacterAlternative,
        digitGroupSeparator: thousandsSymbol,
        decimalPlaces: roundPacking,
        decimalPlacesRawValue: roundPacking,
        decimalPlacesShownOnBlur: roundPacking,
        decimalPlacesShownOnFocus: roundPacking,
        emptyInputBehavior: "zero"
    },
    OptionAutoNumericQuantity: {
        decimalCharacter: decimalSymbol,
        decimalCharacterAlternative: decimalCharacterAlternative,
        digitGroupSeparator: thousandsSymbol,
        decimalPlaces: roundQuantity,
        decimalPlacesRawValue: roundQuantity,
        decimalPlacesShownOnBlur: roundQuantity,
        decimalPlacesShownOnFocus: roundQuantity,
        emptyInputBehavior: "zero"
    },
    OptionAutoNumericAmount: {
        decimalCharacter: decimalSymbol,
        decimalCharacterAlternative: decimalCharacterAlternative,
        digitGroupSeparator: thousandsSymbol,
        decimalPlaces: roundAmount,
        decimalPlacesRawValue: roundAmount,
        decimalPlacesShownOnBlur: roundAmount,
        decimalPlacesShownOnFocus: roundAmount,
        emptyInputBehavior: "zero"
    },
    OptionAutoNumericAmountVND: {
        decimalCharacter: decimalSymbol,
        decimalCharacterAlternative: decimalCharacterAlternative,
        digitGroupSeparator: thousandsSymbol,
        decimalPlaces: roundAmountVND,
        decimalPlacesRawValue: roundAmountVND,
        decimalPlacesShownOnBlur: roundAmountVND,
        decimalPlacesShownOnFocus: roundAmountVND,
        emptyInputBehavior: "zero"
    },
    swal: {
        timer: swalOptions.timer,
        closeOnClickOutside: swalOptions.closeOnClickOutside,
        closeOnEsc: swalOptions.closeOnEsc
    }

}