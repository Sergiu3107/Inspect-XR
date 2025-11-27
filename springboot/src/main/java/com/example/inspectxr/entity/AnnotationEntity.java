// AnnotationEntity.java (Base class)
package com.example.inspectxr.entity;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Inheritance(strategy = InheritanceType.JOINED)
@Data
@Table(name = "annotations")
public abstract class AnnotationEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "created_by", nullable = false)
    private UserEntity createdBy;

    @ManyToOne
    @JoinColumn(name = "model_id", nullable = false)
    private ModelEntity model;

    @ManyToOne
    @JoinColumn(name = "session_id", nullable = false)
    private SessionEntity session;

    @Column(nullable = false, columnDefinition = "TEXT")
    private String data;
}
