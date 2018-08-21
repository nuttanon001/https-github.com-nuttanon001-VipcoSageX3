// angular core
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
// 3rd party
import "rxjs/Rx";
import "hammerjs";
// services
import { DialogsService } from "./shared/dialogs.service";
// modules
import { CustomMaterialModule } from "../shared/customer-material.module";
import { SharedModule } from "../shared/shared.module";
// components
import { ErrorDialog } from "./error-dialog/error-dialog.component";
import { ContextDialog } from "./context-dialog/context-dialog.component";
import { ConfirmDialog } from "./confirm-dialog/confirm-dialog.component";
import { BomDialogComponent } from './bom-dialog/bom-dialog.component';
import { BomTableDialogComponent } from './bom-dialog/bom-table-dialog/bom-table-dialog.component';
import { JobDialogComponent } from './job-dialog/job-dialog.component';
import { JobTableDialogComponent } from './job-dialog/job-table-dialog/job-table-dialog.component';
import { GroupDialogComponent } from './group-dialog/group-dialog.component';
import { GroupTableDialogComponent } from './group-dialog/group-table-dialog/group-table-dialog.component';
import { SupplierDialogComponent } from './supplier-dialog/supplier-dialog.component';
import { BankDialogComponent } from './bank-dialog/bank-dialog.component';
import { SupplierTableDailogComponent } from './supplier-dialog/supplier-table-dailog/supplier-table-dailog.component';
import { BankTableDialogComponent } from './bank-dialog/bank-table-dialog/bank-table-dialog.component';

@NgModule({
  imports: [
    // angular
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    // customer Module
    SharedModule,
    CustomMaterialModule,
  ],
  declarations: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
    BomDialogComponent,
    BomTableDialogComponent,
    JobDialogComponent,
    JobTableDialogComponent,
    GroupDialogComponent,
    GroupTableDialogComponent,
    SupplierDialogComponent,
    BankDialogComponent,
    SupplierTableDailogComponent,
    BankTableDialogComponent,
  ],
  providers: [
    DialogsService,
  ],
  // a list of components that are not referenced in a reachable component template.
  // doc url is :https://angular.io/guide/ngmodule-faq
  entryComponents: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
    BomDialogComponent,
    BomTableDialogComponent,
    JobDialogComponent,
    JobTableDialogComponent,
    GroupDialogComponent,
    GroupTableDialogComponent,
    SupplierDialogComponent,
    BankDialogComponent,
    SupplierTableDailogComponent,
    BankTableDialogComponent,
  ],
})
export class DialogsModule { }
