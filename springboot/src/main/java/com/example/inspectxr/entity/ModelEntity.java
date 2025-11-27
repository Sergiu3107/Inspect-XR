package com.example.inspectxr.entity;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Data
@Table(name = "models")
public class ModelEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String objectUrl;

    @Column(nullable = false)
    private String displayName;

    @ManyToOne
    @JoinColumn(name = "owner_id", nullable = false)
    private UserEntity owner;
}
