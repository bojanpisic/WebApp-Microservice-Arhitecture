import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SystemAdminService } from 'src/services/system-admin.service';
import { CustomValidationService } from 'src/services/custom-validation.service';
import { Address } from 'src/app/entities/address';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-company',
  templateUrl: './add-company.component.html',
  styleUrls: ['./add-company.component.scss']
})
export class AddCompanyComponent implements OnInit {

  adminId: number;
  companyType: string;

  formUser: FormGroup;
  formCompany: FormGroup;

  step = 1;

  errorEmail = false;
  errorUsername = false;
  errorPassword = false;
  errorConfirmPassword = false;
  errorConfirmPasswordMatch = false;
  errorName = false;
  fullAddress: Address;
  address: string;
  lastAddress: string;
  lastGoodAddress: string;
  errorAddress = false;
  errorMsg = '';

  data: {
    email: string,
    username: string,
    password: string,
    confirmPassword: string,
    companyName: string,
    lat: number,
    lon: number,
    city: string,
    state: string,
  };

  constructor(private route: ActivatedRoute, private router: Router,
              public adminService: SystemAdminService, private formBuilder: FormBuilder,
              private customValidator: CustomValidationService,
              private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.adminId = params.id;
      this.companyType = params.type;
    });
  }

  ngOnInit(): void {
    this.initFormUser();
    this.initFormCompany();
  }


  onRegister() {
    if (this.companyType === 'register-airline') {
      console.log(this.data);
      const a = this.adminService.registerAirline(this.data).subscribe(
        (res: any) => {
          this.toastr.success('Success!');
          this.router.navigate(['/system-admin/' + this.adminId]);
        },
        err => {
          console.log(err);
          this.toastr.error(err.error, 'Error.');
        }
      );
    } else {
      const a = this.adminService.registerRACService(this.data).subscribe(
        (res: any) => {
          this.toastr.success('Success!');
          this.router.navigate(['/system-admin/' + this.adminId]);
        },
        err => {
          this.toastr.error(err.error, 'Error.');
        }
      );
    }
  }

  onSubmitUser() {
    if (this.validateUserForm()) {
      this.step = 2;
    }
  }

  onSubmitCompany() {
    if (this.validateCompanyForm()) {
      this.data = {
        email: this.formUser.controls.email.value,
        username: this.formUser.controls.username.value,
        password: this.formUser.controls.password.value,
        confirmPassword: this.formUser.controls.confirmPassword.value,
        companyName: this.formCompany.controls.name.value,
        lat: this.fullAddress.lat,
        lon: this.fullAddress.lon,
        city: this.fullAddress.city,
        state: this.fullAddress.state,
      };
      console.log(this.fullAddress.lon, this.fullAddress.lat);
      this.step = 3;
    }
  }

  onConfirmPassword() {
    this.errorConfirmPasswordMatch = false;
  }

  onExit() {
    this.router.navigate(['/system-admin/' + this.adminId]);
  }

  onBack() {
    this.step--;
  }

  initFormUser() {
    this.formUser = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$')]],
      username: ['', Validators.required],
      password: ['', [Validators.required, this.customValidator.patternValidator()]],
      confirmPassword: ['', Validators.required],
   });
  }

  validateConfirmPassword() {
    return this.formUser.controls.password.value === this.formUser.controls.confirmPassword.value ? true : false;
  }


  initFormCompany() {
    this.formCompany = new FormGroup({
      name: new FormControl('', Validators.required),
   });
  }

  validateUserForm() {
    let retVal = true;
    if (this.formUser.controls.email.invalid) {
      this.errorEmail = true;
      retVal = false;
    }
    if (this.formUser.controls.username.invalid) {
      this.errorUsername = true;
      retVal = false;
    }
    if (this.formUser.controls.password.invalid) {
      this.errorPassword = true;
      retVal = false;
    }
    if (this.formUser.controls.confirmPassword.invalid) {
      this.errorConfirmPassword = true;
      retVal = false;
    }
    if (!this.validateConfirmPassword()) {
      this.errorConfirmPasswordMatch = true;
      retVal = false;
    }
    return retVal;
  }

  validateCompanyForm() {
    let retVal = true;
    if (this.formCompany.controls.name.invalid) {
      this.errorName = true;
      retVal = false;
    }
    if (this.address === undefined || this.lastGoodAddress !== this.lastAddress) {
      if (this.address !== undefined) {
        this.address = undefined;
      }
      this.errorAddress = true;
      retVal = false;
    }
    return retVal;
  }

  onInputChange(value: any) {
    this.lastAddress = value;
  }

  onInput(value: any) {
    const obj = JSON.parse(value);
    this.address = obj.city;
    this.fullAddress = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodAddress = this.lastAddress;
  }

  removeErrorClass() {
    this.errorAddress = false;
  }

}
