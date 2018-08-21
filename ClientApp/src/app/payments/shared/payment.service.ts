import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { Payment } from './payment.model';

@Injectable()
export class PaymentService extends BaseRestService<Payment> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Payment/",
      "PaymentService",
      "PaymentNo",
      httpErrorHandler
    )
  }
}
