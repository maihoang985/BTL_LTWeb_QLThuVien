using System;
using System.Collections.Generic;

namespace Library_Manager.Models;

public partial class TTacGia
{
    public string MaTg { get; set; } = null!;

    public string MaQg { get; set; } = null!;

    public string HoDem { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public virtual TQuocGia MaQgNavigation { get; set; } = null!;

    public virtual ICollection<TTaiLieuTacGia> TTaiLieuTacGia { get; set; } = new List<TTaiLieuTacGia>();
}
