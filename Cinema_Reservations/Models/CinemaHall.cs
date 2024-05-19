using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Reservations.Models
{
	[Serializable]
	public enum HallType
	{
		Small	= 84,			// 6 rzędów po 14 miejsc	= 84 miejsca
		Medium	= 168,			// 12 rzędów po 14 miejsc	= 168 miejsc
		Large	= 252			// 18 rzędów po 14 miejsc	= 252 miejsc
	}

	[Serializable]
	public enum HallColumn
	{
		Columns = 14
	}


	[Serializable]
	public class CinemaHall
	{
		public int Id { get; set; }
		
		[Required]
		public int Number { get; set; }
		
		[Required]
		public HallType Type { get; set; } = HallType.Medium;

		[Required]
		public int Columns { get; set; } = 14;

		[Required]
		public int Rows { get; set; } = 12;

		public CinemaHall() 
		{ }

        public CinemaHall(int id, int number, HallType type)
        {
			Id = id;
			Number = number;
			Type = type;
		}
	}

	/*	Numeracja rzędów:	|	Numeracja miejsc (przykład dla małej sali)
	 *						|
	 *	I					|	|	1	|	2	|	3	|	4	|	5	|	6	|	7	|			| 	8	|	9	|	10	|	11	|	12	|	13	|	14	|
	 *	II					|	|	15	|	16	|	17	|	18	|	19	|	20	|	21	|			| 	22	|	23	|	24	|	25	|	26	|	27	|	28	|
	 *	III					|	|	29	|	30	|	31	|	32	|	33	|	34	|	35	|			| 	36	|	37	|	38	|	39	|	40	|	41	|	42	|
	 *	IV					|	|	43	|	44	|	45	|	46	|	47	|	48	|	49	|			| 	50	|	51	|	52	|	53	|	54	|	55	|	56	|
	 *  V					|	|	57	|	58	|	59	|	60	|	61	|	62	|	63	|			| 	64	|	65	|	66	|	67	|	68	|	69	|	70	|
	 *  VI					|	|	71	|	72	|	73	|	74	|	75	|	76	|	77	|			| 	78	|	79	|	80	|	81	|	82	|	83	|	84	|
	 *  VII					|
	 *  ...				   ...
	 *						
	 * 
	 */
}
