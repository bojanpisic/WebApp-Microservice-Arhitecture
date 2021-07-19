import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SystemAdminService } from 'src/services/system-admin.service';
import { CustomValidationService } from 'src/services/custom-validation.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-system-admin',
  templateUrl: './add-system-admin.component.html',
  styleUrls: ['./add-system-admin.component.scss']
})
export class AddSystemAdminComponent implements OnInit {

  adminId: number;
  companyType: string;

  formUser: FormGroup;

  errorEmail = false;
  errorUsername = false;
  errorPassword = false;
  errorConfirmPassword = false;
  errorConfirmPasswordMatch = false;

  showErrorMsg = true;
  errorMsg = 'Error 404 bla bla';

  data: {
    email: string,
    username: string,
    password: string,
    confirmPassword: string,
  };

  constructor(private route: ActivatedRoute, private router: Router,
              public adminService: SystemAdminService, private formBuilder: FormBuilder,
              private customValidator: CustomValidationService,
              private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.adminId = params.id;
    });
  }

  ngOnInit(): void {
    this.initFormUser();
  }

  onSubmitUser() {
    if (this.validateUserForm()) {
      this.data = {
        email: this.formUser.controls.email.value,
        username: this.formUser.controls.username.value,
        password: this.formUser.controls.password.value,
        confirmPassword: this.formUser.controls.confirmPassword.value,
      };
      this.adminService.registerSystemAdmin(this.data).subscribe(
        (res: any) => {
          this.toastr.success('Success!');
          this.router.navigate(['/system-admin/' + this.adminId]);
        },
        err => {
          this.toastr.error(err.error, 'Error!');
        }
      );
    }
  }

  onConfirmPassword() {
    this.errorConfirmPasswordMatch = false;
  }

  onExit() {
    this.router.navigate(['/system-admin/' + this.adminId]);
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
    console.log('retval' + retVal);
    return retVal;
  }

}
