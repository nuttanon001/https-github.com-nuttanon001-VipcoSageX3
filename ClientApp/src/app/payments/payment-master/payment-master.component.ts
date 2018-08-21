import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from '../../shared/base-schedule.component';
import { Payment } from '../shared/payment.model';
import { PaymentService } from '../shared/payment.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
import { ColumnType, MyPrimengColumn } from '../../shared/column.model';
import { Scroll } from '../../shared/scroll.model';
import { ScrollData } from '../../shared/scroll-data.model';

@Component({
  selector: 'app-payment-master',
  templateUrl: './payment-master.component.html',
  styleUrls: ['./payment-master.component.scss']
})
export class PaymentMasterComponent extends BaseScheduleComponent<Payment,PaymentService> {

  constructor(
    service: PaymentService,
    fb: FormBuilder,
    viewCon: ViewContainerRef,
    serviceDialogs:DialogsService,
  ) {
    super(service, fb, viewCon, serviceDialogs);
  }

  // get request data
  onGetData(schedule: Scroll): void {
    this.service.getAllWithScroll(schedule)
      .subscribe((dbData: ScrollData<Payment>) => {
        if (!dbData) {
          this.totalRecords = 0;
          this.columns = new Array;
          this.datasource = new Array;
          this.reloadData();
          this.loading = false;
          return;
        }

        this.totalRecords = dbData.Scroll.TotalRow || 0;
        // new Column Array
        let width100: number = 100;
        let width150: number = 150;
        let width250: number = 250;
        let width350: number = 350;

        this.columns = new Array;
        this.columns = [
          { field: 'PaymentNo', header: 'PaymentNo.', width: width150,  },
          { field: 'PaymentDateString', header: 'Date.', width: width150,  },
          { field: 'BankNo', header: 'BankNo', width: width150, },
          { field: 'BankName', header: 'BankName', width: width350, },
          { field: 'PayBy', header: 'Pay-By', width: width150, },
          { field: 'SupplierNo', header: 'SupplierNo', width: width150, },
          { field: 'SupplierName', header: 'SupplierName', width: width350, },
          //{ field: 'Currency', header: 'Currency', width: width150, },
          { field: 'AmountString', header: 'Amount', width: width150, },
          { field: 'Description', header: 'Description', width: width250, },
          { field: 'CheckNo', header: 'CheckNo', width: width150, },
          { field: 'RefNo', header: 'RefNo', width: width250, },
        ];

        this.datasource = dbData.Data.slice();
        if (this.needReset) {
          this.first = 0;
          this.needReset = false;
        }

        this.reloadData();
      }, error => {
        this.totalRecords = 0;
        this.columns = new Array;
        this.datasource = new Array;
        this.reloadData();
      }, () => this.loading = false);
  }

  // Open Dialog
  onShowDialog(type?: string): void {
    if (type) {
      if (type === "Bank") {
        this.serviceDialogs.dialogSelectBank(this.viewCon)
          .subscribe(bank => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBank: bank ? bank.BankNumber : undefined,
              BankString: bank ? bank.Description : undefined,
            });
          });
      } else if (type === "Supplier") {
        this.serviceDialogs.dialogSelectSupplier(this.viewCon)
          .subscribe(sub => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereSupplier: sub ? sub.SupplierNo : undefined,
              SupplierString: sub ? sub.SupplierName : undefined,
            });
          });
      } 
    }
  }
}
