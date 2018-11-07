// Angular Core
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Component, OnInit, ViewContainerRef } from '@angular/core';
// Components
import { BaseScheduleComponent } from '../../shared/base-schedule.component';
// Models
import { Payment } from '../shared/payment.model';
import { Scroll } from '../../shared/scroll.model';
import { ScrollData } from '../../shared/scroll-data.model';
import { Bank } from '../../dimension-datas/shared/bank.model';
// Services
import { AuthService } from '../../core/auth/auth.service';
import { PaymentService } from '../shared/payment.service';
import { DialogsService } from '../../dialogs/shared/dialogs.service';

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
    serviceDialogs: DialogsService,
    private serviceAuth: AuthService,
    private router: Router,
  ) {
    super(service, fb, viewCon, serviceDialogs);
  }
  // Parameter
  failLogin: boolean = false;

  ngOnInit(): void {
    this.buildForm();
    if (!this.serviceAuth.getAuth || this.serviceAuth.getAuth.LevelUser < 2) {
      this.serviceDialogs.error("Waining Message", "Access is restricted. please contact administrator !!!", this.viewCon).
        subscribe(() => this.router.navigate(["login"]));
    } else {
      this.failLogin = true;
    }
  }

  // get request data
  onGetData(schedule: Scroll): void {
    if (!this.failLogin) {
      return;
    }

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
        this.serviceDialogs.dialogSelectBank(this.viewCon,1)
          .subscribe((bank: Array<Bank>) => {
            let nameBanks: string = "";
            if (bank) {
              nameBanks = (bank[0].Description.length > 15 ? bank[0].Description.slice(0, 15) + "..." : bank[0].Description) +
                          (bank.length > 1 ? `+ ${bank.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: bank ? bank.map((item) => item.BankNumber) : undefined,
              BankString: bank ? nameBanks : undefined,
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

  // get report data
  onReportPayment(): void {
    if (this.reportForm) {
      this.loading = true;
      const scorll = this.reportForm.getRawValue() as Scroll;
      this.service.getXlsx(scorll).subscribe(data => {
        console.log(data);
        this.loading = false;
      });
      //this.service.getPaymentReport(scorll)
      //  .subscribe(data => {
      //    if (data) {
      //      // console.log(data);
      //      let link: any = document.createElement("a");
      //      link.href = URL.createObjectURL(data);
      //      link.download = "ReportPayment.xlsx";
      //      link.click();
      //    }
      //    // get paper for over time
      //    // this.onGetPaperTaskMachineOverTime(value);
      //  });
    }
  }
}
