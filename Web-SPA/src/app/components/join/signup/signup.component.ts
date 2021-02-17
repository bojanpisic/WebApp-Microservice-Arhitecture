import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/services/user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {

  step = 'step1';
  registrationType: any; // ovde ce da bude sta se registruje..da li airline admin itd..
  adminId: any; //

  errorEmail = false;
  errorUsername = false;
  errorPassword = false;
  errorConfirmPassword = false;
  errorFirstName = false;
  errorLastName = false;
  errorPhone = false;
  errorCity = false;
  errorConfirmPasswordMatch = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              public userService: UserService,
              private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.registrationType = params.registrationType;
      this.adminId = params.id;
    });
   }

  ngOnInit(): void {
    this.userService.formModel.reset();
  }

  next(nextStep: string) {
    if (nextStep === 'step2') {
      if (this.validateFirstStep()) {
        this.step = nextStep;
      }
    } else {
      if (this.validateSecondStep()) {
        this.step = nextStep;
      }
    }
  }

  onConfirmPassword() {
    this.errorConfirmPasswordMatch = false;
  }

  validateConfirmPassword() {
    return this.userService.formModel.controls.Password.value === this.userService.formModel.controls.ConfirmPassword.value ? true : false;
  }

  validateFirstStep() {
    let retVal = true;
    if (this.userService.formModel.controls.FirstName.invalid) {
      this.errorFirstName = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.LastName.invalid) {
      this.errorLastName = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.City.invalid) {
      this.errorCity = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.Phone.invalid) {
      this.errorPhone = true;
      retVal = false;
    }
    return retVal;
  }

  validateSecondStep() {
    let retVal = true;
    if (this.userService.formModel.controls.Email.invalid) {
      this.errorEmail = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.UserName.invalid) {
      this.errorUsername = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.Password.invalid) {
      this.errorPassword = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.ConfirmPassword.invalid) {
      this.errorConfirmPassword = true;
      retVal = false;
    }
    if (!this.validateConfirmPassword()) {
      this.errorConfirmPasswordMatch = true;
      retVal = false;
    }
    return retVal;
  }

  Register() {
    if (this.validateFirstStep() && this.validateSecondStep()) {
      this.userService.userRegistration().subscribe(
        (res: any) => {
            this.userService.formModel.reset();
            this.toastr.success('Success!');
            this.router.navigate(['']);
        },
        err => {
          this.toastr.error(err.error, 'Error.');
        }
      );
    }
  }
}
