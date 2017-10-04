/// <reference path="..\index.html" />
/// <reference path="kendo.web.min.js" />

//#region " constants "

var coos = [];
var items = [];
var brandings = [];
var srcAddresses = [];
var usrPrinter = '';

//#endregion

//#region " Initialize Constants "

function GetLists() {
    $.ajax({
        url: "api/Labels/GetCountryList/",
        type: 'GET',
        data: "{}",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        complete: function (e) {
            if (e.status !== 200) {
                alert(e.responseJSON.Message);
            }
            else {
                FillLists(e.responseJSON);
            }
        }
    });
}

function FillLists(dat) {
    coos.length = 0;
    items.length = 0;
    brandings.length = 0;
    srcAddresses.length = 0;

    //brandings.push({ 'value': '', 'text': 'All'});

    var thCoos = dat[0].Data;
    thCoos.sort(function (a, b) {
        return a.Abbrv > b.Abbrv ? 1 : a.Abbrv < b.Abbrv ? -1 : 0;
    });
    $.each(thCoos, function () {
        var that = this;
        coos.push({ 'id': that.idx, 'Abbr': that.Abbrv, 'FName': that.LongName });
    });

    //var theItems = dat[1].Data;
    //$.each(theItems, function () {
    //    var that = this;
    //    items.push({ 'ItemFull': that.ItemFull, 'BrandFull': that.BrandFull, 'ItemConcat': that.ItemFull + ' ' + that.ItemDesc, 'ItemDesc': that.ItemDesc, 'BrandAbbrv': that.BrandAbbrv, 'GTIN': that.GTIN });
    //});

    var theAddresses = dat[1].Data;
    $.each(theAddresses, function () {
        srcAddresses.push({ 'idx': this.idx, 'Addrs': this.Addrs});
    });

    var theBrands = dat[2];
    $.each(theBrands, function () {
        brandings.push({ 'value': this.toString(), 'text': this.toString() });
    });
}

//#endregion

//#region " Label Code "

// #region old for ref
////OLD method
//function OpenNewLabel() {
//    var tomorrow = new Date();
//    tomorrow.setDate(tomorrow.getDate() + 1);
//    var viewModel = kendo.observable({
//        dataSource: new kendo.data.DataSource({
//            data: [
//                {
//                    idx: -1,
//                    Descrip: '',
//                    SellerName: '',
//                    SoldToName: '',
//                    BtwFile: '',
//                    Barcode: '',
//                    Coo: 0,
//                    UsebyLang: '',
//                    SellOrUseBy: '',
//                    Qty: 1,
//                    LotNum: ''
//                }
//            ]
//        })
//    });
//    var kendoNewLabelWind = $('<div id="divNewLabel"><div id="innerKWind"'
//            + ' data-template="newLabelTemplate" data-bind="source: dataSource" ></div></div>')
//    .kendoWindow({
//        title: 'Create New Label',
//        visible: true,
//        resizable: true,
//        modal: true,
//        width: 500,
//        pinned: true,
//        width: 550,
//        close: function (e) {
//            this.destroy();
//        },
//        actions: ["Close"],
//    });
//    kendo.bind(kendoNewLabelWind, viewModel);
//    var itemBox = kendoNewLabelWind.find('#ddlItemNo');
//    $(itemBox).kendoDropDownList({
//        dataSource: items,
//        dataTextField: "ItemConcat",
//        dataValueField: "ItemId",
//        optionLabel: 'Select...',
//        headerTemplate: '<div class="dropdown-header k-widget k-header" style="font-style: italic">' +
//                                '<span>Item</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
//                                '<span>Description</span>' +
//                            '</div>',
//        valueTemplate: '<span>#:data.ItemFull#</span>',
//        template: '<span>#:data.ItemConcat#</span>',
//        change: function (e) {
//            var itemPick = this.dataItem();
//            $('#txDescrip').val(itemPick.ItemDesc);
//            $('#txSrcAddr').val('Mann Packing Inc., Salinas, CA  93902');
//            $('#txBranding').val(itemPick.BrandFull);
//            $('#txBCode').val(itemPick.GTIN);
//        }
//    });
//    var ddlItems = $('#ddlItemNo').getKendoDropDownList();
//    ddlItems.list.width('400');
//    var dateBox = kendoNewLabelWind.find('#txExp');
//    $(dateBox).kendoDatePicker({
//        format: "MM/dd/yy",
//        min: tomorrow
//    });
//    var qtyBox = kendoNewLabelWind.find('#txQty');
//    $(qtyBox).kendoNumericTextBox({
//        format: "#",
//        decimals: 0,
//        min: 1,
//        value: 1
//    });
//    kendoNewLabelWind.find('#ddlCoo').kendoDropDownList({
//        dataSource: coos,
//        dataTextField: "Abbr",
//        dataValueField: "id",
//        optionLabel: 'Select...'
//    });
//    kendoNewLabelWind.find('#txTemplate').kendoDropDownList({
//        dataSource: btwFileNames,
//        dataTextField: "btwFileName",
//        dataValueField: "Id",
//        optionLabel: 'Select...'
//    });
//    kendoNewLabelWind.data('kendoWindow').center().open();
//    setTimeout(function () {
//        ddlItems.focus();
//    });
//    kendoNewLabelWind.find('#cmCancel').click(function(){
//        $('#divNewLabel').getKendoWindow().close();
//    });
//    kendoNewLabelWind.find('#cmSaveLabel').click(function () {
//        $('#divNewLabel').getKendoWindow().close();
//    });
//    kendoNewLabelWind.find('#cmPrintNow').click(function () {
//        $('#divNewLabel').getKendoWindow().close();
//    });
//}
//#endregion


function FillItemsList() {
    var exisGrid = $('#divItemsList').getKendoGrid();
    if (exisGrid !== undefined) {
        exisGrid.destroy();
        $('#divItemsList').html('');
    }

    $('#divItemsList').kendoGrid({
        sortable: true,
        pageable: true,
        editable: false,
        filterable: {
            mode: "row"
        },
        columns: [
            {
                command: [
                    {
                        name: 'print',
                        text: 'Print...',
                        click: function (e) {
                            var tr = $(e.target).closest("tr");
                            var data = this.dataItem(tr);
                            PrintLabel(data);
                        }
                    }
                ],
                width: 45
            },
            {
                field: 'ItemFull',
                title: 'Item (filter exact)',
                width: 85,
                attributes: {
                    style: "text-align: center;"
                },
                filterable: {
                    extra: false,
                    cell: {
                        operator: 'eq',
                        showOperators: false,
                        dataSource: new kendo.data.DataSource({
                            data: []
                        }),
                    }
                }
            },
            {
                field: 'ItemDesc',
                title: 'Description (filter contains)',
                width: 180,
                filterable: {
                    extra: false,
                    cell: {
                        operator: 'contains',
                        showOperators: false
                    }
                }
            },
            {
                field: 'BrandAbbrv',
                title: 'Brand',
                values: brandings,
                width: 50,
                attributes: {
                    style: "text-align: center;"
                },
                filterable: {
                    extra: false,
                    cell: {
                        template: function (options) {
                            options.element.kendoDropDownList({
                                autoBind: false,
                                dataTextField: "text",
                                dataValueField: "value",
                                valuePrimitive: true,
                                dataSource: brandings,
                                optionLabel: 'All',
                                height: 700
                            });
                        },
                        operator: 'eq',
                        showOperators: false,
                        inputWidth: 70
                    }
                },
                editable: false
            },
            {
                field: 'BrandFull',
                title: 'Brand Full',
                width: 150,
                filterable: false
            },
            {
                field: 'GTIN',
                title: 'Bar Code',
                width: 80,
                filterable: false
            },
            {
                field: 'WalmartCode',
                title: 'Walmart Code',
                width: 80,
                filterable: false
            },
        ],
        dataSource: {
            serverSorting: true,
            serverPaging: true,
            serverFiltering: true,
            pageSize: 16,
            schema: {
                data: 'Data',
                total: 'Total',
                errors: 'error',
                model: {
                    id: "ItemFull",
                    fields: {
                        ItemFull: { type: 'string' },
                        ItemDesc: { type: 'string' },
                        BrandAbbrv: { type: 'string' },
                        BrandFull: { type: 'string' },
                        GTIN: { type: 'string' },
                        WalmartCode: { type: 'string' }
                    }
                }
            },
            transport: {
                read: {
                    url: "api/Labels/GetJdeItemsList/",
                    contentType: "application/json; charset=utf-8",
                    type: "GET",
                    complete: function(e) {
                        if (e.status !== 200) {
                            alert(e.responseJSON.Message);
                        }
                    }
                },
                parameterMap: function (data, operation) {
                    return JSON.stringify(data);
                }
            }
        }
    });

    $("#divItemsList .k-grid-header").css({
        "padding-right": "0px"
    });
    $("#divItemsList").kendoTooltip({
        filter: ".k-grid-print",
        content: "You will be asked for date, quantity and shift before actual printing"
    });
}

function SendPrintJob(data, srcAddr, dPick, dCoo, dQty, shif, selLang, crewNum, incJulian, noDate, isWm, useByDays, walmartCode) {

    var dat = {
        'Id': data.ItemFull,
        'CooId': dCoo,
        'UsebyLang': selLang,
        'Qty': dQty,
        'SellOrUseBy': dPick,
        'Shift': shif,
        'CrewNum': crewNum,
        'SrcAddress': srcAddr,
        'PrinterName': usrPrinter,
        'JulianPlusOne': incJulian,
        'NoDate': noDate,
        'IsWM': isWm,
        'UseByDays': useByDays,
        'WalmartCode': walmartCode
    };

    $.ajax({
        url: "api/Labels/PostPrintLabel/",
        type: 'POST',
        data: JSON.stringify(dat),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        complete: function (e) {
            if (e.status !== 200) {
                alert(e.responseJSON.Message);
            }
        }
    });
}

function PrintLabel(passedData) {
    var isWM = passedData['BrandAbbrv'] === 'WM';
    var tomorrow = new Date();
    //tomorrow.setDate(tomorrow.getDate() + 1);
    var kendoWindow = $('<div id="divPrintdetail"></div>')
    .kendoWindow({
        title: 'Set Date, Shift and Quantity',
        resizable: true,
        modal: true,
        close: function () {
            var that = this;
            setTimeout(function () {
                that.destroy();
            }, 1000)
        }
    });

    if (isWM) {
        $(kendoWindow).data("kendoWindow")
            .content($('#printPrevTemplateWalmart').html())
            .center()
            .open();
    }
    else {
        $(kendoWindow).data("kendoWindow")
            .content($('#printPrevTemplate').html())
            .center()
            .open();
    }



    kendoWindow
        .find("#cmPrintLabel")
        .click(function () {
            var useByDays = -1;
            var noDate = false;
            var incJulian = false;
            if (isWM) {
                useByDays = kendoWindow.find('#txUseByDays').getKendoNumericTextBox().value();
            }
            else {
                noDate = (kendoWindow.find('#ckNoDate')[0]).checked;
                incJulian = (kendoWindow.find('#ckJulianPlusOne')[0]).checked;
            }

            var dPick = kendoWindow.find('#txExp').getKendoDatePicker().value();
            var walmartCode = kendoWindow.find('#txPrevWMCode').val();
            if (dPick === null && noDate === false) {
                alert("Date is required");
                return;
            }

            var shif = $('#tblPrevLabelDetail').find('input[type=radio][name=shifts]:checked').get(0);
            if (shif === undefined) {
                alert("Shift is required");
                return;
            }
            else {
                shif = $('#tblPrevLabelDetail').find('input[type=radio][name=shifts]:checked').get(0).value;
            }
            var cooVal = kendoWindow.find('#ddlCoo').getKendoDropDownList().value();
            var qty = kendoWindow.find('#txQty').getKendoNumericTextBox().value();
            var sellLanguage = kendoWindow.find('#txSellLang').val();
            var crewNum = kendoWindow.find('#txCrewNum').getKendoMaskedTextBox().value();
            var srcAddr = kendoWindow.find('#ddlSrcAddress').getKendoDropDownList().text();
            if (sellLanguage === '') sellLanguage = 'none';
            SendPrintJob(passedData, srcAddr, dPick, cooVal, qty, shif, sellLanguage, crewNum, incJulian, noDate, isWM, useByDays, walmartCode);
        });

    kendoWindow.find("#txPrevItem").val(passedData.ItemFull);
    kendoWindow.find("#txPrevDesc").val(passedData.ItemDesc);
    kendoWindow.find("#txPrevBrand").val(passedData.BrandFull);
    kendoWindow.find("#txPrevGTIN").val(passedData.GTIN);
    kendoWindow.find("#txPrevWMCode").val(passedData.WalmartCode);
    kendoWindow.find("#txSellLang").val('Sell By');

    kendoWindow
        .find("#cmCloseLabel")
            .click(function () {
                $('#divPrintdetail').getKendoWindow().close();
            });

    kendoWindow.find('#ddlCoo').kendoDropDownList({
        dataSource: coos,
        dataTextField: "Abbr",
        dataValueField: "id",
        optionLabel: 'Select...'
    });
    kendoWindow.find('#ddlSrcAddress').kendoDropDownList({
        dataSource: srcAddresses,
        dataTextField: "Addrs",
        dataValueField: "idx",
        value: "1"
    });
    kendoWindow.find('#txExp').kendoDatePicker({
        format: "MM/dd/yy",
        min: tomorrow
    });
    kendoWindow.find('#txQty').kendoNumericTextBox({
        format: "#",
        decimals: 0,
        min: 1,
        value: 1
    });
    kendoWindow.find('#txCrewNum').kendoMaskedTextBox({
        mask: "00"
    });
    if (isWM) {
        kendoWindow.find('#txUseByDays').kendoNumericTextBox({
            format: "#",
            decimals: 0,
            min: 0,
            value: 0
        });
    }
}

//#endregion

//#region " Users "

function SubmitPass() {
    var usr = $('#txUserName').val();
    var pss = $('#txPassword').val();
    var data = { 'LogonName': usr, 'Passworrd': pss };
    kendo.ui.progress($('#divLogon'), true);
    $.ajax({
        url: 'api/Labels/POSTLogin/',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        complete: function (e) {
            if (e.status === 200) {
                $('#divLogon').remove();
                $('#divMainContent').show();
                usrPrinter = e.responseJSON;
                $('#wrpRegister #cmRegister').remove();
                kendo.ui.progress($('#divLogon'), false);
                FillItemsList();
                GetLists();
            }
            else {
                kendo.ui.progress($('#divLogon'), false);
                alert(JSON.parse(e.responseText).Message);
            }
        }
    });
}

function RegisterUser() {
    var txusr = $('#divRegistration #txUserNameReg').val();
    var txPass1 = $('#divRegistration #txPasswordReg').val();
    var txPass2 = $('#divRegistration #txPasswordReg2').val();
    var txEmail = $('#divRegistration #txEmail').val();
    var iPrin = $('#divRegistration #txPrinter').getKendoDropDownList().value();
    if (txusr.length < 4) {
        alert('Logon must be at least four characters');
    }
    else if (txPass1 !== txPass2) {
        alert('Passwords do not match');
    }
    else if (txPass1.length < 4) {
        alert('Password must be at lease four characters');
    }
    else if (iPrin === undefined || iPrin === '') {
        alert('You must specify a printer. If you don\'t know what printer you\'ll be using yet, please register later');
    }
    else {
        var dat = {
            idx: -1,
            Logon: txusr,
            Password: txPass1,
            Printer: iPrin,
            Email: txEmail
        };

        $.ajax({
            url: "api/Labels/PostRegisterUser/",
            type: 'POST',
            data: JSON.stringify(dat),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            complete: function (e) {
                if (e.status !== 200) {
                    alert(e.responseJSON.Message);
                }
                else {
                    $('#divRegistration').hide();
                    $('#divMainContent').show();
                    $('#wrpRegister').remove();
                    usrPrinter = e.responseJSON;
                    FillItemsList();
                    GetLists();
                }
            }

        });
    }
}

//#endregion

//#region " Form Loaded "

$(function () {
    $('#cmRegister').click(function (e) {
        e.preventDefault();
        $('#divLogon').hide();
        $('#divRegistration').show();
        $('#divRegistration #txPrinter').kendoDropDownList({
            dataTextField: 'Printer',
            dataValueField: 'Idx',
            optionLabel: 'Select...',
            dataSource: {
                transport: {
                    read: {
                        url: "api/Labels/GetPrinterList/",
                        dataType: "json"
                    }
                }
            }
        });
        var ddlPrins = $('#divRegistration #txPrinter').getKendoDropDownList();
        ddlPrins.list.width('400');
    });
    $('#form1').submit(function (e) {
        e.preventDefault();
        SubmitPass();
    });
    $('#cmCommit').click(function (e) {
        e.preventDefault();
        SubmitPass();
    });
    $('#txUserName').focus();
    if (window.location.hostname === 'localhost') {
        $('#txUserName').val('rgraham');
        $('#txPassword').val('ghie');
        SubmitPass();
    }
});

//#endregion



