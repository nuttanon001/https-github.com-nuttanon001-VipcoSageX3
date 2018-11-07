import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
// service
import { HttpErrorHandler } from "../../shared/http-error-handler.service";
// models
import { Employee } from "../../employees/shared/employee.model";
// component
import { BaseRestService } from "../../shared/base-rest.service";

@Injectable()
export class EmployeeService extends BaseRestService<Employee> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Employee/",
      "EmployeeService",
      "EmpCode",
      httpErrorHandler
    )
  }
}
