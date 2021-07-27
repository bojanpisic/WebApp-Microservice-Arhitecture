import {
  Directive,
  ElementRef,
  Injectable,
  AfterViewInit,
} from "@angular/core";

@Directive({
  selector: "[appScroll]",
})
@Injectable({
  providedIn: "root",
})
export class ScrollDirective {
  previousScrollHeightMinusTop: number; // the variable which stores the distance
  readyFor: string;
  toReset = false;

  constructor(private elementRef: ElementRef) {
    this.previousScrollHeightMinusTop = 0;
    this.readyFor = "up";
    this.restore();
  }

  ngAfterViewChecked() {}

  reset() {
    this.previousScrollHeightMinusTop = 0;
    this.readyFor = "up";
    this.elementRef.nativeElement.scrollTop =
      this.elementRef.nativeElement.scrollHeight;
    // resetting the scroll position to bottom because that is where chats start.
  }

  restore() {
    if (this.toReset) {
      if (this.readyFor === "up") {
        console.log(this.elementRef.nativeElement.scrollHeight);
        this.elementRef.nativeElement.scrollTop =
          this.elementRef.nativeElement.scrollHeight -
          this.previousScrollHeightMinusTop;
        // restoring the scroll position to the one stored earlier
      }
      this.toReset = false;
    }
  }

  prepareFor(direction) {
    this.toReset = true;
    this.readyFor = direction || "up";
    this.elementRef.nativeElement.scrollTop = !this.elementRef.nativeElement
      .scrollTop // check for scrollTop is zero or not
      ? this.elementRef.nativeElement.scrollTop + 1
      : this.elementRef.nativeElement.scrollTop;
    this.previousScrollHeightMinusTop =
      this.elementRef.nativeElement.scrollHeight -
      this.elementRef.nativeElement.scrollTop;
    // the current position is stored before new messages are loaded
  }
}
