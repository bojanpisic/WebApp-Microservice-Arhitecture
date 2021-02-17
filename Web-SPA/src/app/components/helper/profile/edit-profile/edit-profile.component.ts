import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/entities/user';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/services/user.service';
import { FormGroup, FormControl, Validators, ValidatorFn, AbstractControl} from '@angular/forms';
import { CustomValidationService } from 'src/services/custom-validation.service';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss'],
})
export class EditProfileComponent implements OnInit {

  confirmPasswordType = 'password';
  confirmPasswordPicture = '../../../../../assets/visibility-black-18dp.svg';
  passwordMatch = false;

  showUsername = false;
  showFirstName = false;
  showLastName = false;
  showEMail = false;
  showPassword = false;
  showPhone = false;
  showAddress = false;

  btnUsernameEnabled = true;
  btnFirstNameEnabled = true;
  btnLastNameEnabled = true;
  btnEMailEnabled = true;
  btnPasswordEnabled = true;
  btnPhoneEnabled = true;
  btnAddressEnabled = true;

  form: FormGroup;
  errorPassword = false;
  errorOldPassword = false;
  ok = false;
  address: string;
  lastAddress: string;
  lastGoodAddress: string;
  errorAddress = false;

  userId: any;
  user: {
    username: string,
    firstName: string,
    lastName: string,
    email: string,
    phone: string,
    address: string
  };

  constructor(private route: ActivatedRoute, private userService: UserService, private customValidator: CustomValidationService,
              private router: Router, private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
  }

  goBack() {
    const payLoad = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1]));
    const userRole = payLoad.Roles;
    console.log(userRole);
    if (userRole === 'RegularUser') {
      this.router.navigate(['/' + this.userId + '/profile']);
    } else if (userRole === 'AirlineAdmin') {
      this.router.navigate(['/admin/' + this.userId]);
    } else if (userRole === 'RentACarServiceAdmin') {
      this.router.navigate(['/rac-admin/' + this.userId]);
    } else if (userRole === 'Admin') {
      this.router.navigate(['/system-admin/' + this.userId]);
    }
  }

  ngOnInit(): void {
    const air1 = this.userService.getUser(this.userId).subscribe(
      (data: any) => {
        console.log(data);
        const user = {
          firstName: (data.firstName === null) ? '' : data.firstName,
          lastName: (data.lastName === null) ? '' : data.lastName,
          email: data.email,
          username: data.userName,
          phone: (data.phoneNumber === null) ? '' : data.phoneNumber,
          address: (data.city === null) ? '' : data.city
        };
        this.user = user;
        this.initForm();
        this.ok = true;
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  initForm() {
    this.form = new FormGroup({
      username: new FormControl(this.user.username, Validators.required),
      firstName: new FormControl(this.user.firstName, Validators.required),
      lastName: new FormControl(this.user.lastName, Validators.required),
      email: new FormControl(this.user.email, [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$')]),
      password: new FormControl('', [Validators.required, this.customValidator.patternValidator()]),
      oldPassword: new FormControl('', [Validators.required, this.customValidator.patternValidator()]),
      confirmPassword: new FormControl(''),
      phone: new FormControl(this.user.phone, [Validators.required,
                                               Validators.pattern('^[(][+][0-9]{3}[)][0-9]{2}[/][0-9]{3}[-][0-9]{3,4}')]),
      address: new FormControl(this.user.address, Validators.required),
    });
  }

  editUsername() {
    this.showUsername = !this.showUsername;
    this.btnFirstNameEnabled = !this.btnFirstNameEnabled;
    this.btnLastNameEnabled = !this.btnLastNameEnabled;
    this.btnEMailEnabled = !this.btnEMailEnabled;
    this.btnPasswordEnabled = !this.btnPasswordEnabled;
    this.btnPhoneEnabled = !this.btnPhoneEnabled;
    this.btnAddressEnabled = !this.btnAddressEnabled;
    this.initForm();
  }

  editFirstName() {
    this.showFirstName = !this.showFirstName;
    this.btnUsernameEnabled = !this.btnUsernameEnabled;
    this.btnLastNameEnabled = !this.btnLastNameEnabled;
    this.btnEMailEnabled = !this.btnEMailEnabled;
    this.btnPasswordEnabled = !this.btnPasswordEnabled;
    this.btnPhoneEnabled = !this.btnPhoneEnabled;
    this.btnAddressEnabled = !this.btnAddressEnabled;
    this.initForm();
  }

  editLastName() {
    this.showLastName = !this.showLastName;
    this.btnUsernameEnabled = !this.btnUsernameEnabled;
    this.btnFirstNameEnabled = !this.btnFirstNameEnabled;
    this.btnEMailEnabled = !this.btnEMailEnabled;
    this.btnPasswordEnabled = !this.btnPasswordEnabled;
    this.btnPhoneEnabled = !this.btnPhoneEnabled;
    this.btnAddressEnabled = !this.btnAddressEnabled;
    this.initForm();
  }

  editEMail() {
    this.showEMail = !this.showEMail;
    this.btnUsernameEnabled = !this.btnUsernameEnabled;
    this.btnLastNameEnabled = !this.btnLastNameEnabled;
    this.btnFirstNameEnabled = !this.btnFirstNameEnabled;
    this.btnPasswordEnabled = !this.btnPasswordEnabled;
    this.btnPhoneEnabled = !this.btnPhoneEnabled;
    this.btnAddressEnabled = !this.btnAddressEnabled;
    this.initForm();
  }

  editPassword() {
    this.passwordMatch = false;
    this.errorOldPassword = false;
    this.errorPassword = false;
    this.showPassword = !this.showPassword;
    this.btnUsernameEnabled = !this.btnUsernameEnabled;
    this.btnLastNameEnabled = !this.btnLastNameEnabled;
    this.btnEMailEnabled = !this.btnEMailEnabled;
    this.btnFirstNameEnabled = !this.btnFirstNameEnabled;
    this.btnPhoneEnabled = !this.btnPhoneEnabled;
    this.btnAddressEnabled = !this.btnAddressEnabled;
    this.initForm();
  }

  editPhone() {
    this.showPhone = !this.showPhone;
    this.btnUsernameEnabled = !this.btnUsernameEnabled;
    this.btnLastNameEnabled = !this.btnLastNameEnabled;
    this.btnEMailEnabled = !this.btnEMailEnabled;
    this.btnPasswordEnabled = !this.btnPasswordEnabled;
    this.btnFirstNameEnabled = !this.btnFirstNameEnabled;
    this.btnAddressEnabled = !this.btnAddressEnabled;
    this.initForm();
  }

  editAddress() {
    this.showAddress = !this.showAddress;
    this.btnUsernameEnabled = !this.btnUsernameEnabled;
    this.btnLastNameEnabled = !this.btnLastNameEnabled;
    this.btnEMailEnabled = !this.btnEMailEnabled;
    this.btnPasswordEnabled = !this.btnPasswordEnabled;
    this.btnPhoneEnabled = !this.btnPhoneEnabled;
    this.btnFirstNameEnabled = !this.btnFirstNameEnabled;
    this.initForm();
  }

  saveUsername() {
    if (!this.form.controls.username.invalid) {
      this.user.username = this.form.controls.username.value;
      const data = {
        id: this.userId,
        UserName: this.user.username
      };
      this.userService.changeUsername(data).subscribe(
        (res: any) => {
          this.editUsername();
        },
        err => {
          this.toastr.error(err.error, 'Error!');
        }
      );
    }
  }

  saveFirstName() {
    if (!this.form.controls.firstName.invalid) {
      this.user.firstName = this.form.controls.firstName.value;
      const data = {
        id: this.userId,
        FirstName: this.form.controls.firstName.value
      };
      this.userService.changeFirstName(data).subscribe(
        (res: any) => {
          this.editFirstName();
        },
        err => {
          this.toastr.error(err.error, 'Error!');
        }
      );
    }
  }

  saveLastName() {
    if (!this.form.controls.lastName.invalid) {
      this.user.lastName = this.form.controls.lastName.value;
      const data = {
        id: this.userId,
        LastName: this.form.controls.lastName.value
      };
      this.userService.changeLastName(data).subscribe(
        (res: any) => {
          this.editLastName();
        },
        err => {
          this.toastr.error(err.error, 'Error!');
        }
      );
    }
  }

  saveEMail() {
    if (!this.form.controls.email.invalid) {
      this.user.email = this.form.controls.email.value;
      const data = {
        id: this.userId,
        Email: this.form.controls.email.value
      };
      this.userService.changeEmail(data).subscribe(
        (res: any) => {
          this.editEMail();
          window.location.reload();
        },
        err => {
          this.toastr.error(err.error, 'Error!');
        }
      );
    }
  }

  savePassword() {
    if (this.form.controls.password.value === this.form.controls.confirmPassword.value) {
      if (!this.form.controls.password.invalid && !this.form.controls.oldPassword.invalid) {
        const data = {
          id: this.userId,
          OldPassword: this.form.controls.oldPassword.value,
          Password: this.form.controls.password.value,
          PasswordConfirm: this.form.controls.confirmPassword.value
        };
        this.userService.changePassword(data).subscribe(
          (res: any) => {
            // if (!this.userService.hasChangedPassword()) {
            //   localStorage.removeItem('token');
            //   this.router.navigate(['']);
            // }
            this.editPassword();
            // this.toastr.success('Success!');
            const payLoad = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1]));
            const userRole = payLoad.Roles;
            const passwordChanged = payLoad.PasswordChanged;
            if (userRole === 'AirlineAdmin' && passwordChanged === 'False') {
              localStorage.setItem('token', res.token);
              this.router.navigate(['/admin/' + this.userId]);
            } else if (userRole === 'RentACarServiceAdmin' && passwordChanged === 'False') {
              localStorage.setItem('token', res.token);
              this.router.navigate(['/rac-admin/' + this.userId]);
            } else if (userRole === 'Admin' && passwordChanged === 'False') {
              localStorage.setItem('token', res.token);
              this.router.navigate(['/system-admin/' + this.userId]);
            }
          },
          err => {
            this.toastr.error(err.error, 'Error!');
          }
        );
      }
      if (!this.form.controls.password.invalid) {
        this.errorPassword = true;
      }
      if (!this.form.controls.oldPassword.invalid) {
        this.errorOldPassword = true;
      }
      this.passwordMatch = false;
    } else {
      this.passwordMatch = true;
    }
  }

  savePhone() {
    if (!this.form.controls.phone.invalid) {
      const data = {
        id: this.userId,
        Phone: this.form.controls.phone.value
      };
      this.userService.changePhone(data).subscribe(
        (res: any) => {
          this.user.phone = this.form.controls.phone.value;
          this.editPhone();
        },
        err => {
          this.toastr.error(err.error, 'Error!');
        }
      );
    }
  }

  saveAddress() {
    if (this.address !== undefined) {
      if (this.lastAddress === this.lastGoodAddress) {
        this.user.address = this.address;
        const data = {
          id: this.userId,
          City: this.user.address
        };
        this.userService.changeAddress(data).subscribe(
          (res: any) => {
            this.editAddress();
          },
          err => {
            this.toastr.error(err.error, 'Error!');
          }
        );
      }
    } else {
      this.errorAddress = true;
    }
  }

  toggleEyePicture() {
    this.confirmPasswordType = (this.confirmPasswordType === 'password') ? 'text' : 'password';
    this.confirmPasswordPicture = (this.confirmPasswordPicture === '../../../../../assets/visibility-black-18dp.svg')
                                  ? '../../../../../assets/visibility_off-black-18dp.svg'
                                  : '../../../../../assets/visibility-black-18dp.svg';
  }

  onInputChange(value: any) {
    this.lastAddress = value;
  }

  onInput(value: any) {
    const obj = JSON.parse(value);
    this.address = obj.city + ', ' + obj.state;
    this.lastGoodAddress = this.lastAddress;
  }

  removeErrorClass() {
    this.errorAddress = false;
  }

  onLogOut() {
    localStorage.clear();
    this.router.navigate(['/']);
  }
}
