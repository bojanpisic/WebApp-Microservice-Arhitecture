import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/entities/user';
import { UserService } from 'src/services/user.service';
import * as jwt_decode from 'jwt-decode';
import { DomSanitizer } from '@angular/platform-browser';

class ImageSnippet {
  pending = false;
  status = 'init';

  constructor(public src: string, public file: File) {}
}

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  userId: number;
  user: any;

  formOk = false;

  selectedFile: ImageSnippet;

  imageToShow: any;

  img;

  decoded;

  constructor(private route: ActivatedRoute, private san: DomSanitizer, private userService: UserService,
              private router: Router) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
  }

  private onSuccess() {
    this.selectedFile.pending = false;
    this.selectedFile.status = 'ok';
  }

  private onError() {
    this.selectedFile.pending = false;
    this.selectedFile.status = 'fail';
    this.selectedFile.src = '';
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    const token = localStorage.getItem('token');
    this.decoded = this.getDecodedAccessToken(token);
    const air1 = this.userService.getUser(this.userId).subscribe(
      (data: any) => {
        this.user = {
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          bonus: data.bonus
        };
        this.img = this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${data.imageUrl}`);
        this.formOk = true;
      }
    );
    console.log(air1);
  }

  onFileChanged(imageInput: any) {
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {

      this.selectedFile = new ImageSnippet(event.target.result, file);

      this.selectedFile.pending = true;
      const body = {
        image: this.selectedFile.file,
      };
      this.userService.changePhoto(body).subscribe(
        (res) => {
          this.onSuccess();
        },
        (err) => {
          this.onError();
        });
    });

    reader.readAsDataURL(file);
  }

  onSignOut() {
    localStorage.clear();
    this.router.navigate(['/']);
  }

  onBack() {
    const token = localStorage.getItem('token');
    this.decoded = this.getDecodedAccessToken(token);

    switch (this.decoded.Roles) {
      case 'RegularUser':
        this.router.navigateByUrl(this.decoded.UserID + '/home');
        break;
      case 'AirlineAdmin':
        if (this.decoded.PasswordChanged === 'True') {
          this.router.navigateByUrl('/admin/' + this.decoded.UserID);
        } else {
          this.router.navigateByUrl('/admin/' + this.decoded.UserID + '/profile/edit-profile');
        }
        break;
      case 'RentACarServiceAdmin':
        console.log(this.decoded.PasswordChanged);
        if (this.decoded.PasswordChanged === 'True') {
          this.router.navigateByUrl('/rac-admin/' + this.decoded.UserID);
        } else {
          this.router.navigateByUrl('/rac-admin/' + this.decoded.UserID + '/profile/edit-profile');
        }
        break;
      case 'Admin':
        this.router.navigateByUrl('/system-admin/' + this.decoded.UserID);
        break;
    }
  }

  getDecodedAccessToken(token: string): any {
    try {
        return jwt_decode(token);
    } catch (Error) {
        return null;
    }
  }
}
