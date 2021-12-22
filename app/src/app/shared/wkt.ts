export class Wkt {

  public constructor(wkt: string, srid: number) {
    this.Wkt = wkt;
    this.Srid = srid;
  }

  Wkt: string;
  Srid: number;
}
