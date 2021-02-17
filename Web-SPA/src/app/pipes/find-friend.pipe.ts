import { Pipe, PipeTransform } from '@angular/core';
import { User } from '../entities/user';
import { UserService } from 'src/services/user.service';

@Pipe({
  name: 'findFriend'
})
export class FindFriendPipe implements PipeTransform {
  transform(users: Array<User>, inputString?: string): Array<User> {
    if (inputString === '') {
      return users;
    }
    const retVal = new Array<User>();
    users.forEach(user => {
      if (user.firstName.toLowerCase().startsWith(inputString.toLowerCase())
         || user.lastName.toLowerCase().startsWith(inputString.toLowerCase())) {
        if (!retVal.includes(user)) {
          retVal.push(user);
        }
      }
    });
    return retVal;
  }

}
