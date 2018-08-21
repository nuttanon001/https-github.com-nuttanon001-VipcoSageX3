import { Injectable } from '@angular/core';
import { PrAndPo } from './pr-and-po.model';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PrService extends BaseRestService<PrAndPo> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseRequest/",
      "PurchaseRequestService",
      "PrNumber",
      httpErrorHandler
    )
  }
}
