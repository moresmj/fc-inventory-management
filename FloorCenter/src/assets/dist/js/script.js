$(document).ready(function() {
        $('#dataTables-example').DataTable({
            responsive: true
        });
        // $(".calendar").click(function() {
        //       $("#orDate").daterangepicker({
        //         singleDatePicker: true,
        //         showDropdowns: true
        //     });
        // });

        //Stock Registration
         $('#s_return').click(function() {
            $("#approve_code").toggle(this.checked);
        });

        //Stock Registration, Stock List, Stock Receive, Delivery Request, Sales Registration, Sales List
        //DATEPICKER
        
        $("#rDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#rDate2").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#poDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#poDate2").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#drDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#drDate2").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#orDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#orDate2").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#toDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });

        
        $("#sadeliveryDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#deliveryDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#wadeliveryDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });
        $("#wdeliveryDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
        });

        //Set Delivery Request

       //Add Store Delivery modal
        $('#store_delivery_add[name="store_delivery_add"]').change(function() {
            var deliveryStatus = $(this).find(":selected").text().trim();
            if (deliveryStatus.length === 0){
                $("#store_deli_add1").hide();
                $("#store_deli_add2").hide();
            } else if (deliveryStatus.length >= 1 && deliveryStatus.length <= 3){
                console.log(deliveryStatus.length + "New");
                $("#store_deli_add1").show();
                $("#store_deli_add2").hide();
            } else if (deliveryStatus.length >= 5){
                console.log(deliveryStatus.length + "Other");
                $("#store_deli_add1").hide();
                $("#store_deli_add2").show();
            }
        });

        //Edit Store Delivery modal
        $('#store_delivery_edit[name="store_delivery_edit"]').change(function() {
            var deliveryStatus = $(this).find(":selected").text().trim();
            if (deliveryStatus.length === 0){
                $("#store_deli_edit1").hide();
                $("#store_deli_edit2").hide();
            } else if (deliveryStatus.length >= 1 && deliveryStatus.length <= 3){
                console.log(deliveryStatus.length + "New");
                $("#store_deli_edit1").show();
                $("#store_deli_edit2").hide();
            } else if (deliveryStatus.length >= 5){
                console.log(deliveryStatus.length + "Other");
                $("#store_deli_edit1").hide();
                $("#store_deli_edit2").show();
            }
        });

        //Add Warehouse Delivery modal
        $('#warehouse_delivery_add[name="warehouse_delivery_add"]').change(function() {
            var deliveryStatus = $(this).find(":selected").text().trim();
            if (deliveryStatus.length === 0){
                $("#warehouse_deli_add1").hide();
                $("#warehouse_deli_add2").hide();
            } else if (deliveryStatus.length >= 1 && deliveryStatus.length <= 3){
                console.log(deliveryStatus.length + "New");
                $("#warehouse_deli_add1").show();
                $("#warehouse_deli_add2").hide();
            } else if (deliveryStatus.length >= 5){
                console.log(deliveryStatus.length + "Other");
                $("#warehouse_deli_add1").hide();
                $("#warehouse_deli_add2").show();
            }
        });

        //Edit Warehouse Delivery modal
        $('#warehouse_delivery_edit[name="warehouse_delivery_edit"]').change(function() {
            var deliveryStatus = $(this).find(":selected").text().trim();
            if (deliveryStatus.length === 0){
                $("#warehouse_deli_edit1").hide();
                $("#warehouse_deli_edit2").hide();
            } else if (deliveryStatus.length >= 1 && deliveryStatus.length <= 3){
                console.log(deliveryStatus.length + "New");
                $("#warehouse_deli_edit1").show();
                $("#warehouse_deli_edit2").hide();
            } else if (deliveryStatus.length >= 5){
                console.log(deliveryStatus.length + "Other");
                $("#warehouse_deli_edit1").hide();
                $("#warehouse_deli_edit2").show();
            }
        });

        //branch name
        $(".dropdown > .dropdown-branch li").click(function() {
            var branch = $(this).text();
            $(".branch-name").text(branch);
            console.log(branch);
        });

        //Stock Request Registration
        $('#transaction[name="transaction"]').change(function() {
            var transaction = $(this).find(":selected").text().trim();
            if (transaction === "Request"){
                $("#po_num").attr('disabled','disabled');
                $("#vendor").attr('disabled','disabled');
                $("#poDate").attr('disabled','disabled');
                console.log("disabled")
            } else {
                $("#po_num").removeAttr('disabled');
                $("#vendor").removeAttr('disabled');
                $("#poDate").removeAttr('disabled');
                console.log("not disabled");
            }
        });

        function hideLabel( btn ){
          $(".tbl_input").show();
          $(".tbl_label").hide();
          console.log("label hide");
        }

        function showLabel( btn ){
          $(".tbl_input").hide();
          $(".tbl_label").show();
          console.log("label show");
        }

        $('#tbl_update').click(function(){
          $(this).toggleClass('btn_show');
          return $(this).hasClass('btn_show') ? hideLabel(this) : showLabel(this);
        });

        function hideLabel2( btn ){
          $(".tbl_input2").show();
          $(".tbl_label2").hide();
          console.log("label hide");
        }

        function showLabel2( btn ){
          $(".tbl_input2").hide();
          $(".tbl_label2").show();
          console.log("label show");
        }

        $('#tbl_update2').click(function(){
          $(this).toggleClass('btn_show2');
          return $(this).hasClass('btn_show2') ? hideLabel2(this) : showLabel2(this);
        });

        function showLoader( btn ){
          $(".loader").show();
          console.log("label hide");
        }

        function hideLoader( btn ){
          $(".loader").hide();
          console.log("lodal show");
        }

        $('#loader_modal').click(function(){
          $(this).toggleClass('btn_show');
          return $(this).hasClass('btn_show') ? showLoader(this) : hideLoader(this);
        });


    });