import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OldWay } from './old-way';

describe('OldWay', () => {
  let component: OldWay;
  let fixture: ComponentFixture<OldWay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OldWay]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OldWay);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
