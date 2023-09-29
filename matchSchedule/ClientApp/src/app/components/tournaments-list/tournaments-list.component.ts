import { Component, OnInit } from '@angular/core';
import { TournamentService } from 'src/app/services/tournament.service';
import { Tournament } from 'src/app/shared/tournament';

@Component({
  selector: 'app-tournaments-list',
  templateUrl: './tournaments-list.component.html',
  styleUrls: ['./tournaments-list.component.css']
})
export class TournamentsListComponent implements OnInit {
  tournaments: Tournament[] = [];
  constructor(private service: TournamentService) {}
  panelOpenState = false;

  ngOnInit(): void {
    this.service.loadTournaments().subscribe((data) => {
      this.tournaments = data;
    });
  }
}