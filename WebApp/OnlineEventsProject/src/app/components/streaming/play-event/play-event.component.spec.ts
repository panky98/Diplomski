import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayEventComponent } from './play-event.component';

describe('PlayEventComponent', () => {
  let component: PlayEventComponent;
  let fixture: ComponentFixture<PlayEventComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlayEventComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlayEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
