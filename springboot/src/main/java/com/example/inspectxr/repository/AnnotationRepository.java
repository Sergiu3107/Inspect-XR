package com.example.inspectxr.repository;

import com.example.inspectxr.entity.AnnotationEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface AnnotationRepository extends JpaRepository<AnnotationEntity, Long> {

    Optional<AnnotationEntity> findById(Long id);
}
