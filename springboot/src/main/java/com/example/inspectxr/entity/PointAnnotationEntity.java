package com.example.inspectxr.entity;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Data
@Table(name = "point_annotations")
public class PointAnnotationEntity extends AnnotationEntity {

    @Column(nullable = false)
    private float posX;

    @Column(nullable = false)
    private float posY;

    @Column(nullable = false)
    private float posZ;
}
