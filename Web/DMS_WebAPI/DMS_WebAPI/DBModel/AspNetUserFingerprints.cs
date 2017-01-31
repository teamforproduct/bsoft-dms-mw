﻿using DMS_WebAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserFingerprints
    {
        public int Id { get; set; }

        [Index("IX_UserFingerprint", 1, IsUnique = true)]
        [Index("IX_UserName", 1, IsUnique = true)]
        public string UserId { get; set; }

        /// <summary>
        /// Пользовательское наименование отпечатка
        /// </summary>
        [MaxLength(2000)]
        [Index("IX_UserName", 2, IsUnique = true)]
        public string Name { get; set; }

        /// <summary>
        /// Отпечаток
        /// </summary>
        [MaxLength(2000)]
        [Index("IX_UserFingerprint", 2, IsUnique = true)]
        public string Fingerprint { get; set; }

        /// <summary>
        /// Браузер
        /// </summary>
        [MaxLength(2000)]
        public string Browser { get; set; }

        /// <summary>
        /// Операционная система
        /// </summary>
        [MaxLength(2000)]
        public string Platform { get; set; }
        public bool IsActive{ get; set; }

        public DateTime LastChangeDate { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

    }
}