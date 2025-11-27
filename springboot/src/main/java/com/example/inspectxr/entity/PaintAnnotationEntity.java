package com.example.inspectxr.entity;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Data
@Table(name = "paint_annotations")
public class PaintAnnotationEntity extends AnnotationEntity {

    @Column(nullable = false)
    private String imageLayer;

    @Column(nullable = false)
    private String color;
}
